using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel; // 일시정지 화면 UI
    [SerializeField] private GameObject optionPanel; // 옵션 화면 UI
    [SerializeField] private QueenEnhanceStatusUI queenEnhanceStatusUI; // 여왕 강화 스탯창

    [SerializeField] private Button pauseButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;

    public bool isPaused = false;

    /// <summary>
    /// 초기화 및 버튼 이벤트 등록
    /// </summary>
    private void Awake()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (optionPanel != null)
            optionPanel.SetActive(false);

        if (pauseButton != null)
            pauseButton.gameObject.SetActive(true);

        pauseButton.onClick.AddListener(TogglePause);
        continueButton.onClick.AddListener(TogglePause);
        optionButton.onClick.AddListener(OnClickOption);
        exitButton.onClick.AddListener(OnClickActiveResult);
    }

    /// <summary>
    /// GameManager에 본 PauseController 등록
    /// </summary>
    private void Start()
    {
        GameManager.Instance.SetPauseController(this);
    }

    /// <summary>
    /// 옵션 버튼 클릭 시 옵션 패널 표시
    /// </summary>
    public void OnClickOption()
    {
        optionPanel.SetActive(true);
    }

    /// <summary>
    /// 타이틀 화면으로 이동
    /// </summary>
    public void OnClickActiveResult()
    {
        TogglePause();
        GameResultManager.Instance.DisplayGameResult();
    }

    /// <summary>
    /// 일시정지 상태를 토글합니다.
    /// </summary>
    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    /// <summary>
    /// 외부에서 강제로 일시정지 상태로 만듭니다.
    /// </summary>
    public void ForcePause()
    {
        if (!isPaused)
            Pause();
    }

    /// <summary>
    /// 게임을 일시정지합니다.
    /// </summary>
    private void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        queenEnhanceStatusUI.RefreshStatus();
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// 게임을 재개합니다.
    /// </summary>
    private void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Confined;
    }

    /// <summary>
    /// 현재 일시정지 상태인지 반환합니다.
    /// </summary>
    public bool IsPaused()
    {
        return isPaused;
    }
}
