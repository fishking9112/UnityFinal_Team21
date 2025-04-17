using UnityEngine;

public class MonsterSlotUI : BaseSlotUI<MonsterInfo>
{
    // Queen의 몬스터 슬롯에 정보 추가
    public override void AddSlot(int index, MonsterInfo monster)
    {
        base.AddSlot(index,monster);

        if (index < 0 || index >= slotIcon.Count)
        {
            return;
        }

        slotIcon[index].sprite = DataManager.Instance.iconData.GetSprite(monster.outfit);
        slotIcon[index].enabled = true;
        slotIcon[index].preserveAspect = true;

    }
}