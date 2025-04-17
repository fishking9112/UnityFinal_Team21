using UnityEngine;

public class MonsterSlotUI : BaseSlotUI<MonsterInfo>
{
    protected override void OnSlotAdd(int index, MonsterInfo monster)
    {
        if (index < slotIcon.Count)
        {
            slotIcon[index].sprite = DataManager.Instance.iconData.GetSprite(monster.outfit);
            slotIcon[index].enabled = true;
            slotIcon[index].preserveAspect = true;
        }
    }
}