using Cysharp.Threading.Tasks;
using UnityEngine;

public enum CursorState
{
    CONFINED,
    NONE,
}

public class GameManager : MonoSingleton<GameManager>
{
    public int Gold { get; private set; }

    public Queen queen;
    public Castle castle;
    private CursorState curCursorState;
    private MainUI mainUI;
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
        if (Input.GetKeyDown(KeyCode.G))
        {
            AddGold(100);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            UGSManager.Instance.SaveLoad.SaveAsync().Forget();
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
        // if (pause)
        // {
        //     // if (GameManager.Instance.IsPaused()) return;

        //     if (pauseController != null)
        //     {
        //         pauseController.ForcePause();
        //     }
        //     //_ = UGSManager.Instance.SaveLoad.SaveAsync();
        // }
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


    public void AddGold(int amount)
    {
        Gold += amount;
        RefreshGoldText();
    }

    public bool TrySpendGold(int amount)
    {
        if (Gold >= amount)
        {
            Gold -= amount;
            RefreshGoldText();
            return true;
        }
        return false;
    }

    public void SetGold(int amount)
    {
        Gold = Mathf.Max(0, amount);
        RefreshGoldText();
    }

    public int GetGold()
    {
        return Gold;
    }

    private void RefreshGoldText()
    {
        if (mainUI != null)
        {
            mainUI.RefreshGoldText();
        }
    }

    public void SetMainUI(MainUI mainUI)
    {
        this.mainUI = mainUI;
    }

    public bool IsPaused()
    {
        return pauseController.isPaused;
    }
}
