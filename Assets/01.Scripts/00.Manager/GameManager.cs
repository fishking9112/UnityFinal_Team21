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
    }

    private void Update()
    {
        ApplyCursorState();
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
            if(pauseController != null)
            {
                pauseController.ForcePause();
            }
            //_ = UGSManager.Instance.SaveLoad.SaveAsync();
        }
    }

    public void GameClear()
    {
        // 결과창 켜기
    }

    public void GameOver()
    {
        // 결과창 켜기
    }
}
