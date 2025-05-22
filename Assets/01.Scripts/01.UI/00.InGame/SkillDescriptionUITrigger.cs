using UnityEngine;
using UnityEngine.EventSystems;

public class SkillDescriptionUITrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public QueenActiveSkillBase skill;
    public SkillDescriptionUI descriptionUI;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(GameManager.Instance.queen.controller.curSlot == QueenSlot.QueenActiveSkill)
        {
            if (skill != null)
            {
                var info = skill.info;

                descriptionUI.ShowUI(
                    DataManager.Instance.iconAtlas.GetSprite(info.icon),
                    info.name,
                    info.description,
                    $"{info.coolTime}초",
                    $"{info.cost}"
                );
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionUI.HideUI();
    }
}
