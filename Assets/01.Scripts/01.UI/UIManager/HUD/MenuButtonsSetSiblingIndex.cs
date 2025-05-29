using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonsSetSiblingIndex : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private MenuHUD menuHUD;
    [SerializeField] private Transform panel;
    [SerializeField] private Image imageBtn;
    [SerializeField] private Image panelBtn;
    [SerializeField] private Image imageOutlineImage;
    [SerializeField] private Transform tfButton;

    private Color outlineColorMouseOver = new Color(1f, 1f, 1f);          // 흰색
    private Color outlineColorMouseOut = new Color(90f / 255f, 90f / 255f, 90f / 255f);  // 회색

    private Button imagebtn;
    private Button panelbtn;
    private RectTransform rectPanel;
    private Vector2 panelOriginalSize;

    private void Awake()
    {
        rectPanel = panel.GetComponent<RectTransform>();
        panelOriginalSize = rectPanel.sizeDelta;
        rectPanel.sizeDelta = new Vector2(0f, panelOriginalSize.y);
        panel.gameObject.SetActive(false);

        imagebtn = imageBtn.GetComponent<Button>();
        panelbtn = panelBtn.GetComponent<Button>();
        imageOutlineImage.color = outlineColorMouseOut;

        imagebtn.onClick.AddListener(HandleButtonClick);
        panelbtn.onClick.AddListener(HandleButtonClick);
    }
    private void HandleButtonClick()
    {
        OnPointerExit(null);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        menuHUD.BlackBackground.SetAsLastSibling();
        menuHUD.BlackBackground.gameObject.SetActive(true);
        tfButton.SetAsLastSibling();
        panel.gameObject.SetActive(true);

        imageOutlineImage.color = outlineColorMouseOver;

        rectPanel.DOKill();
        rectPanel.sizeDelta = new Vector2(0f, panelOriginalSize.y);
        rectPanel.DOSizeDelta(new Vector2(200f, panelOriginalSize.y), 1.5f).SetEase(Ease.OutExpo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectPanel.DOKill();
        rectPanel.sizeDelta = new Vector2(0f, panelOriginalSize.y);

        imageOutlineImage.color = outlineColorMouseOut;

        menuHUD.BlackBackground.SetAsFirstSibling();
        menuHUD.BlackBackground.gameObject.SetActive(false);
        panel.gameObject.SetActive(false);
    }
}
