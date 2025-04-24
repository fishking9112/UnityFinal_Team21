using UnityEngine;


[RequireComponent(typeof(PauseUI))]
public class PauseController : MonoBehaviour
{
    [SerializeField] private PauseUI ui;

    private void Awake()
    {
        ui.continueButton.onClick.AddListener(CloseUI);
        ui.optionButton.onClick.AddListener(OnClickOption);
        ui.exitButton.onClick.AddListener(OnClickGameResult);
    }

    public void OnEnable()
    {
        ui.RefreshStatus();
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
