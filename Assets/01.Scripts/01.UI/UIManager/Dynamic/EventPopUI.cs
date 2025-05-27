using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;

public class EventPopUI : BaseUI
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Image iconImg;

    [SerializeField] private CanvasGroup canvasGroup; // 반드시 할당
    private CancellationTokenSource fadeCts;

    /// <summary>
    /// 팝업 초기화
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();
        canvasGroup.alpha = 0f;
    }

    /// <summary>
    /// 팝업 설정
    /// </summary>
    public void Setup(string title, string icon)
    {
        if (titleText != null)
            titleText.text = title;

        var iconSprite = DataManager.Instance.iconAtlas.GetSprite(icon);
        if (iconSprite != null)
            iconImg.sprite = iconSprite;

        fadeCts?.Cancel();
        fadeCts?.Dispose();
        fadeCts = new CancellationTokenSource();
        ShowSequenceAsync().Forget();
    }

    public override void OnHide()
    {
        fadeCts?.Cancel();
        fadeCts?.Dispose();
        fadeCts = null;

        base.OnHide();
    }

    /// <summary>
    /// 비동기 FadeIn → 대기 → FadeOut 시퀀스
    /// </summary>
    public async UniTask ShowSequenceAsync(float fadeDuration = 0.5f, float holdDuration = 1.5f)
    {
        await FadeInAsync(fadeDuration, fadeCts.Token);
        await UniTask.Delay(TimeSpan.FromSeconds(holdDuration), cancellationToken: fadeCts.Token, ignoreTimeScale: true);
        await FadeOutAsync(fadeDuration, fadeCts.Token);

        OnHide();
    }

    private async UniTask FadeInAsync(float duration, CancellationToken token)
    {
        canvasGroup.blocksRaycasts = true;
        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(time / duration);
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
        canvasGroup.alpha = 1f;
    }

    private async UniTask FadeOutAsync(float duration, CancellationToken token)
    {
        canvasGroup.blocksRaycasts = false;
        float time = 0f;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            canvasGroup.alpha = Mathf.Clamp01(1f - (time / duration));
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
        canvasGroup.alpha = 0f;
    }
}
