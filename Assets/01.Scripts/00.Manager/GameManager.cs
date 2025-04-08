using UnityEngine;

public enum CursorState
{
    CONFINED,
    NONE,
}

public class GameManager : MonoSingleton<GameManager>
{
    public Queen queen;
    private CursorState curCursorState;

    protected override void Awake()
    {
        curCursorState = CursorState.CONFINED;
    }

    private void Update()
    {
        ApplyCursorState();

        if (UnityEngine.Input.GetKeyDown(KeyCode.A))
        {
            _ = UGSManager.Instance.Leaderboard.UploadScoreAsync(50);
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.S))
        {
            _ = UGSManager.Instance.Leaderboard.UploadScoreAsync(100);
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.G))
        {
            //_ = UGSManager.Instance.SaveLoad.LoadAsync();
            _ = UGSManager.Instance.Leaderboard.GetMyRankAsync();
        }
        if (UnityEngine.Input.GetKeyDown(KeyCode.T))
        {
            _ = UGSManager.Instance.Leaderboard.GetTop10ScoresAsync();
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


    private async void OnApplicationQuit()
    {
        await UGSManager.Instance.SaveLoad.SaveAsync();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            _ = UGSManager.Instance.SaveLoad.SaveAsync();
        }
    }
}
