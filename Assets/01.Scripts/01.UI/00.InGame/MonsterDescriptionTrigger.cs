using UnityEngine;
using UnityEngine.EventSystems;

public class MonsterDescriptionTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MonsterInfo monster;
    public MonsterDescriptionUI descriptionUI;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.Instance.queen.controller.curSlot == QueenSlot.MONSTER)
        {
            if (monster != null)
            {
                descriptionUI.ShowUI(
                    DataManager.Instance.iconAtlas.GetSprite(monster.icon),
                    monster.name,
                    monster.description,
                    $"{monster.cost}"
                );
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionUI.HideUI();
    }

    private void OnDisable()
    {
        descriptionUI.HideUI();
    }
}
