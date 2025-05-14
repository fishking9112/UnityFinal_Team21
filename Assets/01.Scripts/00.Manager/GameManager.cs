using Cysharp.Threading.Tasks;
using UnityEngine;

public enum CursorState
{
    CONFINED,
    NONE,
}

public class GameManager : MonoSingleton<GameManager>
{
    public ReactiveProperty<int> Gold { get; private set; } = new();

    public Queen queen;
    public Castle castle;
    private CursorState curCursorState;
    public CameraController cameraController;

    // 게임 시작 시 시간에 관한 변수들
    public float gameLimitTime = 1800f;
    public bool isTimeOver = true;
    public ReactiveProperty<float> curTime = new ReactiveProperty<float>();


    // private PauseController pauseController;


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

        OnTimer(); // TODO : 임시로 달아둠 나중에 반드시 옮기기
    }

    private void OnTimer() // TODO : 임시로 달아둠 나중에 반드시 옮기기
    {
        if (isTimeOver) return;

        curTime.Value -= Time.deltaTime;

        if (curTime.Value <= 0f)
        {
            GameClear();
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

    // 씬로드에서 불러내기?
    public void GameStart()  // TODO : 임시로 달아둠 나중에 반드시 옮기기(?) 이건 미정
    {
        curTime.Value = gameLimitTime;
        isTimeOver = false;
    }

    public void GameClear()
    {
        curTime.Value = 0f;
        isTimeOver = true;
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().ShowWindow<GameResultUI>();
        // Time.timeScale = 0f;
    }

    public void GameOver()
    {
        curTime.Value = 0f;
        isTimeOver = true;
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().ShowWindow<GameResultUI>();
        // Time.timeScale = 0f;
    }

    public bool TrySpendGold(int amount)
    {
        if (Gold.Value >= amount)
        {
            Gold.Value -= amount;
            return true;
        }
        return false;
    }

    public void AddGold(int amount)
    {
        Gold.Value += amount;
    }

    public void SetGold(int amount)
    {
        Gold.Value = Mathf.Max(0, amount);
    }

    public int GetGold()
    {
        return Gold.Value;
    }
}
