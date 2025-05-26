using UnityEngine;
using UnityEngine.UI;

public class PauseUI : SingleUI
{
    [Header("UIs")]
    [SerializeField] private QueenEnhanceStatusUI queenEnhanceStatusUI;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button continueButton;


    public RectTransform cameraRect;

    private void Awake()
    {
        exitButton.onClick.AddListener(OnClickGameResult);
        continueButton.onClick.AddListener(OnClickContinue);
    }

    private void OnEnable()
    {
        RefreshStatus();
    }

    /// <summary>
    /// 
    /// 강화 상태 새로고침
    /// </summary>
    public void RefreshStatus()
    {
        queenEnhanceStatusUI.RefreshStatus();
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    private void OnClickGameResult()
    {
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().HideWindow();
        GameManager.Instance.GameOver();
    }

    /// <summary>
    /// 계속 하기 버튼 눌렀을 때
    /// </summary>
    private void OnClickContinue()
    {
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().HideWindow();
    }
}
