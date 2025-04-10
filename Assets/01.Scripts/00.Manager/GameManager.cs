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
    public Hero hero;

    protected override void Awake()
    {
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
