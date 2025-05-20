using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OwnedEnhanceItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject selectedUI;
    [SerializeField] private Image enhanceIcon;
    private QueenEnhanceUI queenEnhanceUI;
    private int enhanceID;

    public void SetEnhanceItem(int enhanceID)
    {
        this.enhanceID = enhanceID;
        queenEnhanceUI = StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().queenEnhanceUI;

        enhanceIcon.sprite = DataManager.Instance.iconAtlas.GetSprite(DataManager.Instance.queenEnhanceDic[enhanceID].Icon);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        queenEnhanceUI.QueenEnhanceStatusUI.DescriptionPopupUI.SetActive(true);
        queenEnhanceUI.QueenEnhanceStatusUI.SetDescriptionPopupUIInfo(enhanceID);
        queenEnhanceUI.QueenEnhanceStatusUI.FollowMouse().Forget();
        selectedUI.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        queenEnhanceUI.QueenEnhanceStatusUI.DescriptionPopupUI.SetActive(false);
        selectedUI.SetActive(false);
    }
}
