using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI; // 일시정지 화면 UI

    private bool isPaused = false;

    private void Awake()
    {
        if (pauseUI != null)
            pauseUI.SetActive(false);
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

        if (pauseUI != null)
        {
            pauseUI.SetActive(true);


        }
        Cursor.lockState = CursorLockMode.None;
    }

    private void Resume()
    {
        isPaused = false;
        Time.timeScale = 1f;

        if (pauseUI != null)
            pauseUI.SetActive(false);

        Cursor.lockState = CursorLockMode.Confined;
    }

    public bool IsPaused()
    {
        return isPaused;
    }
}
