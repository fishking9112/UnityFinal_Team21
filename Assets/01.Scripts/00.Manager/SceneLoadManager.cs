using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LoadSceneEnum
{
    LoginScene,
    MenuScene,
    GameScene,
}

public class SceneLoadManager : MonoSingleton<SceneLoadManager>
{
    private LoadingUI loadingUI => StaticUIManager.Instance.loadingUI;
    private bool isSceneLoading = false;
    void Start()
    {
        LoadScene(LoadSceneEnum.LoginScene).Forget();
    }

    public async UniTask LoadScene(LoadSceneEnum sceneEnum)
    {
        if (isSceneLoading)
        {
            Utils.LogError($"이미 씬을 로딩중에 있기 때문에 해당 씬을 로드할 수 없습니다 : {sceneEnum}");
            return;
        }

        isSceneLoading = true;

        switch (sceneEnum)
        {
            case LoadSceneEnum.LoginScene: // 로그인 씬 일 경우
                await loadingUI.Show(); // 로딩창 나타내기 (기본 값 0.5초)
                await LoadSceneAsync("LoginScene"); // 로드 씬으로 이동
                await AddressableManager.Instance.InitDownloadAsync(); // Addressable 다운로드
                await UGSManager.Instance.InitAsync(); // UGS 초기화
                isSceneLoading = false; // 여기서 다른 씬을 로드하기 위해 필요
                await LoadScene(LoadSceneEnum.MenuScene); // 다 됬으면 메뉴 씬으로 이동
                break;
            case LoadSceneEnum.MenuScene: // 메뉴 씬 일 경우
                await loadingUI.Show(); // 로딩창 나타내기 (기본 값 0.5초)
                await LoadSceneAsync("MenuScene"); // 메뉴 씬으로 이동
                await StaticUIManager.Instance.LoadUI(LoadSceneEnum.MenuScene);
                UIManager.Instance.ShowTooltip((int)IDToolTip.MainMenu);
                await UniTask.Delay(1000, DelayType.UnscaledDeltaTime); // 1초 기다리기
                await loadingUI.Hide(); // 로딩창 사라지기 (기본 값 0.5초)
                break;
            case LoadSceneEnum.GameScene: // 게임 씬 일 경우
                await loadingUI.Show(); // 로딩창 나타내기 (기본 값 0.5초)
                await LoadSceneAsync("GameScene");
                await StaticUIManager.Instance.LoadUI(LoadSceneEnum.GameScene);

                // 시간 멈춤
                Time.timeScale = 0f;
                StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().isPaused = true;

                // 만약 OpenWindow가 없다면 시간 흐르게 하기
                UIManager.Instance.ShowTooltip((int)IDToolTip.InGame, onFinishAction: () =>
                {
                    if (StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().openWindow == null)
                    {
                        Time.timeScale = 1f; // 시간 흐름
                        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().isPaused = false;
                    }
                });
                await UniTask.Delay(1000, DelayType.UnscaledDeltaTime); // 1초 기다리기
                await loadingUI.Hide(); // 로딩창 사라지기 (기본 값 0.5초)
                GameManager.Instance.GameStart(); // 게임 스타트(?)
                break;
            default:
                // await LoadSceneAsync("Error"); // 에러씬으로 이동(?)
                Utils.LogError($"해당 이름을 가진 씬은 없습니다");
                break;
        }

        isSceneLoading = false;
    }

    private bool IsValidScene(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (name == sceneName)
                return true;
        }
        return false;
    }

    /// <summary>
    /// 씬을 비동기로 로드
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
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
}
