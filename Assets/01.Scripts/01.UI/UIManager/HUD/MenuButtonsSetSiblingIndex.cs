using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonsSetSiblingIndex : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private MenuHUD menuHUD;
    [SerializeField] private Transform panel;
    [SerializeField] private Material outlineWhite;
    [SerializeField] private Material outlineGray;
    [SerializeField] private Image imageBtn;
    [SerializeField] private Image panelBtn;
    [SerializeField] private Transform tfButton;

    private Button imagebtn;
    private Button panelbtn;
    private RectTransform rectPanel;
    private Vector2 panelOriginalSize;

    private void Awake()
    {
        imageBtn.material = outlineGray;
        // imageBtn.alphaHitTestMinimumThreshold = 0.2f;
        rectPanel = panel.GetComponent<RectTransform>();
        panelOriginalSize = rectPanel.sizeDelta;
        rectPanel.sizeDelta = new Vector2(0f, panelOriginalSize.y);
        panel.gameObject.SetActive(false);
        imagebtn = imageBtn.GetComponent<Button>();
        panelbtn = panelBtn.GetComponent<Button>();

        imagebtn.onClick.AddListener(HandleButtonClick);
        panelbtn.onClick.AddListener(HandleButtonClick);
    }
    private void HandleButtonClick()
    {
        OnPointerExit(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        imageBtn.material = outlineWhite;
        menuHUD.BlackBackground.SetAsLastSibling();
        menuHUD.BlackBackground.gameObject.SetActive(true);
        tfButton.transform.SetAsLastSibling();
        panel.gameObject.SetActive(true);

        rectPanel.DOKill();
        rectPanel.sizeDelta = new Vector2(0f, panelOriginalSize.y);

        rectPanel.DOSizeDelta(new Vector2(200f, panelOriginalSize.y), 1.5f).SetEase(Ease.OutExpo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        imageBtn.material = outlineGray;

        rectPanel.DOKill();
        rectPanel.sizeDelta = new Vector2(0f, panelOriginalSize.y);

        menuHUD.BlackBackground.SetAsFirstSibling();
        menuHUD.BlackBackground.gameObject.SetActive(false);
        panel.gameObject.SetActive(false);
    }
}
