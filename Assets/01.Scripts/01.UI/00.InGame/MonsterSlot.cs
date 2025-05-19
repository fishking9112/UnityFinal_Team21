using UnityEngine;

public class MonsterSlot : BaseSlot<MonsterInfo>
{
    // Queen의 몬스터 슬롯에 정보 추가
    public override void AddSlot(int index, MonsterInfo monster)
    {
        base.AddSlot(index,monster);

        if (index < 0 || index >= slotIconList.Count)
        {
            return;
        }

        slotIconList[index].sprite = DataManager.Instance.iconAtlas.GetSprite(monster.outfit);
        slotIconList[index].enabled = true;
        slotIconList[index].preserveAspect = true;

        // 해당 슬롯에 몬스터 설명을 위한 스킬 정보 넣기
        var trigger = slotIconList[index].GetComponent<MonsterDescriptionTrigger>();
        if (trigger != null)
        {
            trigger.monster = monster;
        }
    }
}