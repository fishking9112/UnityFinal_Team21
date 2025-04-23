using UnityEngine;


[RequireComponent(typeof(PauseUI))]
public class PauseController : MonoBehaviour
{
    [SerializeField] private PauseUI ui;
    [SerializeField] private QueenEnhanceStatusUI queenEnhanceStatusUI;


    private void Awake()
    {
        ui.continueButton.onClick.AddListener(CloseUI);
        ui.optionButton.onClick.AddListener(OnClickOption);
        ui.exitButton.onClick.AddListener(OnClickGameResult);
    }

    private void CloseUI()
    {
        InGameUIManager.Instance.HideWindow();
    }

    public void OnClickOption()
    {
        ui.SetOptionPanel(true);
    }

    public void OnClickGameResult()
    {
        InGameUIManager.Instance.HideWindow();
        InGameUIManager.Instance.ShowWindow<GameResultController>();
    }
}
