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
    }
}