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

    private void Start()
    {
        GameManager.Instance.SetPauseController(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void OnClickOption()
    {
        optionPanel.SetActive(true);
    }

    public void OnClickGoToTitle()
    {
        Time.timeScale = 1f;
        SceneLoadManager.Instance.LoadScene("MainUITest").Forget();
    }

    public void TogglePause()
    {
        if (isPaused)
            Resume();
        else
            Pause();
    }

    public void ForcePause()
    {
        if (!isPaused)
            Pause();
    }

    private void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        queenEnhanceStatusUI.RefreshStatus();
        Cursor.lockState = CursorLockMode.None;
    }

    private void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Confined;
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
