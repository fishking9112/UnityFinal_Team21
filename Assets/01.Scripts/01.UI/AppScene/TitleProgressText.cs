using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class TitleProgressText : MonoBehaviour
{
    [SerializeField] private GameObject UIGroup;
    [SerializeField] private TextMeshProUGUI progressText;

    private bool isAnimatingLoadingText = false;

    private CancellationTokenSource cts;

    private string textValue;

    private void Awake()
    {
        UIGroup.SetActive(false);
    }

    private void OnEnable()
    {
        SceneLoadManager.Instance.titleProgressText = this;
    }

    private void OnDisable()
    {
        StopAnimating();
        UIGroup.SetActive(false);
    }

    public void ActiveUIGroup(bool value)
    {
        UIGroup.SetActive(value);
    }

    public void SetAnimText(string textValue)
    {
        this.textValue = textValue;
    }

    public void StartAnimating()
    {
        if (isAnimatingLoadingText)
            return;

        AnimateLoadingText().Forget();
    }

    public void StopAnimating()
    {
        isAnimatingLoadingText = false;
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
        progressText.text = "";
    }

    private async UniTaskVoid AnimateLoadingText()
    {
        isAnimatingLoadingText = true;
        cts = CancellationTokenSource.CreateLinkedTokenSource(this.GetCancellationTokenOnDestroy());

        string baseText = textValue;
        string[] dots = { ".", "..", "...", "" };
        int index = 0;

        try
        {
            while (isAnimatingLoadingText && !cts.Token.IsCancellationRequested)
            {
                progressText.text = baseText + dots[index];
                index = (index + 1) % dots.Length;

                await UniTask.Delay(500, DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update, cts.Token);
            }
        }
        catch (OperationCanceledException)
        {
            // 애니메이션 중 취소된 경우
        }

        progressText.text = "";
    }
}
