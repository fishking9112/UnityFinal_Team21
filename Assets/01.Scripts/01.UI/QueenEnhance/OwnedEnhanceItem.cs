using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OwnedEnhanceItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject selectedUI;
    [SerializeField] private Image enhanceIcon;

    private QueenEnhanceUI queenEnhanceUI;
    private GameResultUI gameResultUI;

    private int enhanceID;
    private bool isResult;

    public void SetEnhanceItem(int enhanceID, bool isResult)
    {
        this.isResult = isResult;
        this.enhanceID = enhanceID;

        // 아이콘 설정
        var iconName = DataManager.Instance.queenEnhanceDic[enhanceID].Icon;
        enhanceIcon.sprite = DataManager.Instance.iconAtlas.GetSprite(iconName);

        // UI 캐싱
        if (isResult)
        {
            gameResultUI = StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().gameResultUI;
        }
        else
        {
            queenEnhanceUI = StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().queenEnhanceUI;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isResult)
        {
            gameResultUI.DescriptionPopupUI.SetActive(true);
            gameResultUI.SetDescriptionPopupUIInfo(enhanceID);
            gameResultUI.FollowMouse().Forget();
        }
        else
        {
            var statusUI = queenEnhanceUI.QueenEnhanceStatusUI;
            statusUI.DescriptionPopupUI.SetActive(true);
            statusUI.SetDescriptionPopupUIInfo(enhanceID);
            statusUI.FollowMouse().Forget();
        }

        selectedUI.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isResult)
        {
            gameResultUI.DescriptionPopupUI.SetActive(false);
        }
        else
        {
            queenEnhanceUI.QueenEnhanceStatusUI.DescriptionPopupUI.SetActive(false);
        }

        selectedUI.SetActive(false);
    }
}
