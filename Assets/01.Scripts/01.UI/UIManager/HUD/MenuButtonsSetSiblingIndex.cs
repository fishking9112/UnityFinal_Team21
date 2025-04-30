using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonsSetSiblingIndex : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private MenuHUD menuHUD;
    [SerializeField] private Transform panel;
    [SerializeField] private Material outline;
    [SerializeField] private Material outlineAlpha;
    [SerializeField] private Image imageBtn;

    private Button btn;
    private RectTransform rectPanel;
    private Vector2 panelOriginalSize;

    private void Awake()
    {
        imageBtn.material = outlineAlpha;
        imageBtn.alphaHitTestMinimumThreshold = 0.2f;
        rectPanel = panel.GetComponent<RectTransform>();
        panelOriginalSize = rectPanel.sizeDelta;
        rectPanel.sizeDelta = new Vector2(0f, panelOriginalSize.y);
        panel.gameObject.SetActive(false);
        btn = imageBtn.GetComponent<Button>();

        btn.onClick.AddListener(HandleButtonClick);
    }
    private void HandleButtonClick()
    {
        OnPointerExit(null); 
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        imageBtn.material = outline;
        menuHUD.BlackBackground.SetAsLastSibling();
        menuHUD.BlackBackground.gameObject.SetActive(true);
        transform.SetAsLastSibling();
        panel.gameObject.SetActive(true);

        rectPanel.DOKill();
        rectPanel.sizeDelta = new Vector2(0f, panelOriginalSize.y);

        rectPanel.DOSizeDelta(new Vector2(400f, panelOriginalSize.y), 1.5f).SetEase(Ease.OutExpo);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        imageBtn.material = outlineAlpha;

        rectPanel.DOKill();
        rectPanel.sizeDelta = new Vector2(0f, panelOriginalSize.y);

        menuHUD.BlackBackground.SetAsFirstSibling();
        menuHUD.BlackBackground.gameObject.SetActive(false);
        panel.gameObject.SetActive(false);
    }
}
