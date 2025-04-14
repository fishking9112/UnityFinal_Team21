using UnityEngine;

public class MonsterSlotUI : BaseSlotUI<TestMonster>
{
    protected override void OnSlotAdd(int index, TestMonster monster)
    {
        if (index < slotIcon.Count)
        {
            slotIcon[index].sprite = monster.icon;
            slotIcon[index].enabled = true;
            slotIcon[index].preserveAspect = true;
        }
    }

    // 키값으로 sprite 가져오기
    public override Sprite GetIcon(string name)
    {
        TestMonster monster = slotList.Find(monster => monster.name == name);
        return monster != null ? monster.icon : null;
    }
}
