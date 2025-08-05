using UnityEngine;
using UnityEngine.EventSystems;

public class TopButtonDescriptionUITrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public IDUIToolTip id;
    public TopButtonDescriptionUI topButtonDescriptionUI;

    public void OnPointerEnter(PointerEventData eventData)
    {
        topButtonDescriptionUI.ShowUI(
            DataManager.Instance.uiToolTipDic[(int)id].name,
            DataManager.Instance.uiToolTipDic[(int)id].description
        );
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        topButtonDescriptionUI.HideUI();
    }

    private void OnDisable()
    {
        topButtonDescriptionUI.HideUI();
    }
}
