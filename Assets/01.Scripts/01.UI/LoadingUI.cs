using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private TextMeshProUGUI loadingText;

    private bool isAnimatingLoadingText = false;
    private bool isFadeAction = false;

    public async UniTask Show(float fadeDuration = 0.5f)
    {
        if (gameObject.activeSelf || isFadeAction) // 이미 활성화 중이거나 페이드 중이면 무시
            return;

        isFadeAction = true;

        gameObject.SetActive(true);
        loadingText.text = "";

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.interactable = true;
        fadeCanvasGroup.blocksRaycasts = true;

        AnimateLoadingText();
        await FadeInAsync(fadeDuration);
        isFadeAction = false;
    }

    public async UniTask Hide(float fadeDuration = 0.5f)
    {
        if (!gameObject.activeSelf || isFadeAction) // 비활성화 중이거나 페이드 중이면 무시
            return;

        isFadeAction = true;

        isAnimatingLoadingText = false;
        await FadeOutAsync(fadeDuration);

        gameObject.SetActive(false);
        isFadeAction = false;
    }

    private async UniTask FadeInAsync(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / duration); // 0 -> 1로 페이드 인
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        fadeCanvasGroup.alpha = 1f; // 최종 알파 값 보장
    }

    private async UniTask FadeOutAsync(float duration)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / duration); // 1 -> 0으로 페이드 아웃
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        fadeCanvasGroup.alpha = 0f; // 최종 알파 값 보장
        fadeCanvasGroup.interactable = false;
        fadeCanvasGroup.blocksRaycasts = false;
    }

    private void AnimateLoadingText()
    {
        isAnimatingLoadingText = true;

        UniTask.Void(async () =>
        {
            string baseText = "Loading";
            string[] dots = { ".", "..", "...", "" };
            int index = 0;

            while (isAnimatingLoadingText)
            {
                loadingText.text = baseText + dots[index];
                index = (index + 1) % dots.Length;
                await UniTask.Delay(500, DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update);
            }

            loadingText.text = ""; // 종료 시 초기화
        });
    }
}