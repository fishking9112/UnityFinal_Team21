using UnityEngine;

public enum CursorState
{
    CONFINED,
    NONE,
}

public class GameManager : MonoSingleton<GameManager>
{
    public Queen queen;
    public Castle castle;
    private CursorState curCursorState;
    private PauseController pauseController;

    protected override void Awake()
    {
        base.Awake();

        curCursorState = CursorState.CONFINED;
        Time.timeScale = 1f;
    }

    private void Update()
    {
        ApplyCursorState();

        if (Input.GetKeyDown(KeyCode.H))
        {
            castle.TakeDamaged(100f);
        }
    }

    private void ApplyCursorState()
    {
        switch (curCursorState)
        {
            case CursorState.CONFINED:
                Cursor.lockState = CursorLockMode.Confined;
                break;
            case CursorState.NONE:
                Cursor.lockState = CursorLockMode.None;
                break;
        }
    }

    public void SetPauseController(PauseController pauseController)
    {
        this.pauseController = pauseController;
    }

    private async void OnApplicationQuit()
    {
        //await UGSManager.Instance.SaveLoad.SaveAsync();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (pauseController != null)
            {
                pauseController.ForcePause();
            }
            //_ = UGSManager.Instance.SaveLoad.SaveAsync();
        }
    }

    public void GameClear()
    {
        GameObject.Find("GameResultCanvas").transform.Find("ResultWindow").gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void GameOver()
    {
        GameObject.Find("GameResultCanvas").transform.Find("ResultWindow").gameObject.SetActive(true);
        Time.timeScale = 0f;
    }
}
