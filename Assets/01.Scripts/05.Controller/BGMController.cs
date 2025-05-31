using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System;
using UnityEngine.SceneManagement;

[Serializable]
public class SceneBGMInfo
{
    public List<string> sceneNames;
    public List<AudioClip> bgmClips;
}

public class BGMController : MonoBehaviour
{
    [SerializeField] private List<SceneBGMInfo> sceneBGM;

    private string curScene;
    private string curBGM;
    private bool isLooping;

    private CancellationTokenSource cts;
    private CancellationTokenSource oneShotCts;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var nextScene = scene.name;

        var currentGroup = sceneBGM.Find(g => g.sceneNames.Contains(curScene));
        var nextGroup = sceneBGM.Find(g => g.sceneNames.Contains(nextScene));

        bool sameGroup;

        if (currentGroup != null && nextGroup != null && currentGroup == nextGroup)
        {
            sameGroup = true;
        }
        else
        {
            sameGroup = false;
        }

        if (!sameGroup)
        {
            isLooping = false;
            cts?.Cancel();
            cts?.Dispose();

            if (nextGroup != null && nextGroup.bgmClips.Count > 0)
            {
                currentGroup = nextGroup;
                PlaySceneBGMLoop(currentGroup).Forget();
            }
        }

        curScene = nextScene;
    }

    private async UniTaskVoid PlaySceneBGMLoop(SceneBGMInfo bgmInfo)
    {
        isLooping = true;
        cts = new CancellationTokenSource();

        if (bgmInfo == null || bgmInfo.bgmClips.Count == 0)
        {
            return;
        }

        while (isLooping)
        {
            var nextClip = GetRandomClip(bgmInfo.bgmClips);

            if (!SoundManager.Instance.TryGetClip(nextClip.name, out var resolvedClip))
            {
                break;
            }

            curBGM = nextClip.name;

            SoundManager.Instance.ChangeBGM(curBGM);

            try
            {
                await UniTask.Delay((int)(resolvedClip.length * 1000), cancellationToken: cts.Token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }
    }

    private AudioClip GetRandomClip(List<AudioClip> clips)
    {
        var temp = clips.FindAll(c => c.name != curBGM);

        if (temp.Count == 0)
        {
            return clips[0];
        }

        return temp[UnityEngine.Random.Range(0, temp.Count)];
    }

    public async UniTask PlayOneShotBGM(string clipName)
    {
        isLooping = false;
        cts?.Cancel();
        cts?.Dispose();

        oneShotCts?.Cancel();
        oneShotCts = new CancellationTokenSource();

        if (!SoundManager.Instance.TryGetClip(clipName, out var clip))
        {
            return;
        }

        SoundManager.Instance.ChangeBGM(clip.name);

        try
        {
            await UniTask.Delay((int)(clip.length * 1000), cancellationToken: oneShotCts.Token);
        }
        catch (OperationCanceledException)
        {

        }

        var group = sceneBGM.Find(g => g.sceneNames.Contains(curScene));

        if (group != null)
        {
            PlaySceneBGMLoop(group).Forget();
        }
    }

    public void StopOneShotBGM()
    {
        oneShotCts?.Cancel();
    }
}