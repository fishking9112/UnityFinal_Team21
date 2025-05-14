using UnityEngine;
using UnityEngine.UI;

public class PauseUI : SingleUI
{
    [Header("패널들")]
    public GameObject optionPanel;
    [SerializeField] private QueenEnhanceStatusUI queenEnhanceStatusUI;

    [Header("UI 버튼들")]
    public RectTransform cameraRect;

    [Header("UI 버튼들")]
    public Button continueButton;
    public Button optionButton;
    public Button exitButton;

    private void Awake()
    {
        continueButton.onClick.AddListener(CloseUI);
        optionButton.onClick.AddListener(OnClickOption);
        exitButton.onClick.AddListener(OnClickGameResult);
    }

    private void OnEnable()
    {
        RefreshStatus();
    }

    /// <summary>
    /// 옵션 패널 활성화/비활성화
    /// </summary>
    public void SetOptionPanel(bool active)
    {
        optionPanel?.SetActive(active);
    }

    /// <summary>
    /// 강화 상태 새로고침
    /// </summary>
    public void RefreshStatus()
    {
        queenEnhanceStatusUI.RefreshStatus();
    }

    /// <summary>
    /// 일시정지 창 닫기
    /// </summary>
    private void CloseUI()
    {
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().HideWindow();
    }

    /// <summary>
    /// 옵션 버튼 눌렀을 때
    /// </summary>
    private void OnClickOption()
    {
        SetOptionPanel(true);
    }

    /// <summary>
    /// 게임 종료 버튼 눌렀을 때
    /// </summary>
    private void OnClickGameResult()
    {
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().HideWindow();
        GameManager.Instance.GameOver();
        // StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().ShowWindow<GameResultUI>();
    }
}
