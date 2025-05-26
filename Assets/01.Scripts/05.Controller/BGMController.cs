using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System;

public class BGMController : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips; // 인스펙터에서 직접 넣을 BGM 클립들
    private List<string> bgmNames = new List<string>();
    private string currentBGM;
    private bool isLooping = true;
    private CancellationTokenSource cts;

    private void Awake()
    {
        // 클립 이름들을 자동으로 추출
        foreach (var clip in audioClips)
        {
            if (clip != null)
                bgmNames.Add(clip.name);
        }
    }

    private void Start()
    {
        PlayRandomBGMLoop(GetRandomBGMName()).Forget();
    }

    private async UniTaskVoid PlayRandomBGMLoop(string nextBGM)
    {
        cts = new CancellationTokenSource();

        bool isFirst = true;
        isLooping = true;

        while (isLooping)
        {
            if (!isFirst)
                nextBGM = GetRandomBGMName();
            else
                isFirst = false;

            if (!SoundManager.Instance.TryGetClip(nextBGM, out var clip))
            {
                Debug.LogWarning($"SoundManager에 '{nextBGM}'이 로드되어 있지 않습니다.");
                break;
            }

            currentBGM = nextBGM;
            SoundManager.Instance.ChangeBGM(currentBGM);

            try
            {
                await UniTask.Delay((int)(clip.length * 1000), cancellationToken: cts.Token);
            }
            catch (OperationCanceledException)
            {
                continue;
            }
        }
    }

    public void ChangeTo(string newBGM)
    {
        if (!bgmNames.Contains(newBGM)) return;

        cts?.Cancel();
        cts?.Dispose();
        isLooping = false;

        PlayRandomBGMLoop(newBGM).Forget();
    }

    private string GetRandomBGMName()
    {
        if (bgmNames.Count == 0) return null;

        string newBGM;
        do
        {
            newBGM = bgmNames[UnityEngine.Random.Range(0, bgmNames.Count)];
        } while (newBGM == currentBGM && bgmNames.Count > 1);

        return newBGM;
    }
}
