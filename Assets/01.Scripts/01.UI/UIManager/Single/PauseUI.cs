using UnityEngine;
using UnityEngine.UI;

public class PauseUI : SingleUI
{
    [Header("UIs")]
    [SerializeField] private QueenEnhanceStatusUI queenEnhanceStatusUI;
    public Button exitButton;


    public RectTransform cameraRect;

    private void Awake()
    {
        exitButton.onClick.AddListener(OnClickGameResult);
    }

    private void OnEnable()
    {
        RefreshStatus();
    }

    /// <summary>
    /// 강화 상태 새로고침
    /// </summary>
    public void RefreshStatus()
    {
        queenEnhanceStatusUI.RefreshStatus();
    }

    /// <summary>
    /// 게임 종료 버튼 눌렀을 때
    /// </summary>
    private void OnClickGameResult()
    {
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().HideWindow();
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().ShowWindow<GameResultUI>();
    }
}
