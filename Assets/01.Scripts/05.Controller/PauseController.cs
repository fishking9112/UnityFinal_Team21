using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel; // 일시정지 화면 UI
    [SerializeField] private GameObject optionPanel; // 옵션 화면 UI
    [SerializeField] private QueenEnhanceStatusUI queenEnhanceStatusUI; // 여왕 강화 스탯창

    [SerializeField] private Button continueButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button exitButton;

    private bool isPaused = false;

    /// <summary>
    /// 초기화 및 버튼 이벤트 등록
    /// </summary>
    private void Awake()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (optionPanel != null)
            optionPanel.SetActive(false);

        continueButton.onClick.AddListener(TogglePause);
        optionButton.onClick.AddListener(OnClickOption);
        exitButton.onClick.AddListener(OnClickGoToTitle);
    }

    /// <summary>
    /// GameManager에 본 PauseController 등록
    /// </summary>
    private void Start()
    {
        GameManager.Instance.SetPauseController(this);
    }

    /// <summary>
    /// ESC 키 입력 시 일시정지 토글
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
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
    /// TODO : 결과창 만들어지면 결과창으로 연결하기
    /// </summary>
    public void OnClickGoToTitle()
    {
        Time.timeScale = 1f;
        SceneLoadManager.Instance.LoadScene("MainUITest").Forget();
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
