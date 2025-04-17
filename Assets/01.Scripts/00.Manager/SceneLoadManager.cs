using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : MonoSingleton<SceneLoadManager>
{
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private TextMeshProUGUI loadingText;

    private bool isFading = false; 
    private bool isAnimatingLoadingText = false;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    /// <summary>
    /// 참조 객체 상태 초기화
    /// </summary>
    private void Initialize()
    {
        loadingCanvas.SetActive(false);
        fadeCanvasGroup.gameObject.SetActive(true);
        loadingText.gameObject.SetActive(true);
    }

    /// <summary>
    /// 페이드 아웃 -> 로딩 UI -> 씬 로드 -> 페이드 인의 순서로 씬을 전환
    /// </summary>
    /// <param name="sceneName">로드할 씬 이름</param>
    public async UniTask LoadScene(string sceneName)
    {
        if (isFading) return;

        isFading = true;

        fadeCanvasGroup.interactable = true;
        fadeCanvasGroup.blocksRaycasts = true;

        loadingText.text = "";
        loadingCanvas.SetActive(true);        // 로딩 UI 켜기
        await FadeOutAsync();                 // 화면 어두워짐
        AnimateLoadingText();                 // 점 애니메이션 시작

        await LoadSceneAsync(sceneName);      // 씬 로드
        await UniTask.Delay(3000);            // 1초 대기

        fadeCanvasGroup.interactable = false;
        fadeCanvasGroup.blocksRaycasts = false;

        isAnimatingLoadingText = false;       // 점 애니메이션 종료
        await UniTask.Delay(500);             // 점 애니메이션 종료 대기

        await FadeInAsync();                  // 화면 밝아짐
        loadingCanvas.SetActive(false);       // 로딩 UI 끄기

        isFading = false;
    }

    /// <summary>
    /// 비동기로 씬을 로드하며, 활성화 시점을 직접 제어함
    /// </summary>
    /// <param name="sceneName">로드할 씬 이름</param>
    private async UniTask LoadSceneAsync(string sceneName)
    {
        var loadOp = SceneManager.LoadSceneAsync(sceneName);
        loadOp.allowSceneActivation = false;

        while (loadOp.progress < 0.9f)
        {
            await UniTask.Yield();
        }

        loadOp.allowSceneActivation = true;
        await UniTask.NextFrame();
    }

    /// <summary>
    /// 화면을 점점 어둡게 만드는 페이드 아웃 효과
    /// </summary>
    /// <param name="duration">페이드에 걸리는 시간</param>
    private async UniTask FadeOutAsync(float duration = 0.5f)
    {
        fadeCanvasGroup.gameObject.SetActive(true);
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Clamp01(t / duration);
            await UniTask.Yield();
        }

        fadeCanvasGroup.alpha = 1f;
    }

    /// <summary>
    /// 화면을 점점 밝게 만드는 페이드 인 효과
    /// </summary>
    /// <param name="duration">페이드에 걸리는 시간</param>
    private async UniTask FadeInAsync(float duration = 0.5f)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            fadeCanvasGroup.alpha = 1f - Mathf.Clamp01(t / duration);
            await UniTask.Yield();
        }

        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.gameObject.SetActive(false);
    }

    /// <summary>
    /// "Loading..." 점 애니메이션 효과
    /// </summary>
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
                await UniTask.Delay(500);
            }

            loadingText.text = ""; // 종료 시 초기화
        });
    }
}
