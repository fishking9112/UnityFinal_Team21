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
        UIManager.Instance.ShowPopup("알림", "정말로 로비로 돌아가시겠습니까?", 
                                    () =>
                                    {
                                        // 게임 HUD 비활성화
                                        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().HideWindow();

                                        // 게임 종료 처리
                                        GameManager.Instance.GameOver();
                                    }, () => { Utils.Log("취소."); });
    }

    /// <summary>
    /// 계속 하기 버튼 눌렀을 때
    /// </summary>
    private void OnClickContinue()
    {
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().HideWindow();
    }
}
