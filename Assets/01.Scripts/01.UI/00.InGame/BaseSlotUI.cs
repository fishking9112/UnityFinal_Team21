using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class BaseSlotUI : MonoBehaviour
{
    [SerializeField] protected List<Image> slotIcon;

    protected List<TestMonster> monsterList = new List<TestMonster>();

    // 슬롯에 추가
    public virtual void AddSlot(TestMonster monster)
    {
        if (monsterList.Contains(monster))
        {
            return;
        }

        monsterList.Add(monster);

        int index = monsterList.Count - 1;

        if(index < slotIcon.Count)
        {
            slotIcon[index].sprite = monster.icon;
            slotIcon[index].enabled = true;
            slotIcon[index].preserveAspect = true;
        }
    }

    // 슬롯에서 제거
    public virtual void RemoveSlot(TestMonster monster)
    {
        int index = monsterList.IndexOf(monster);

        if(index >= 0 && index < slotIcon.Count)
        {
            slotIcon[index].sprite = null;
            slotIcon[index].enabled = false;
        }

        monsterList.Remove(monster);
    }

    // 슬롯의 번호로 해당 슬롯의 key값 가져오기
    public TestMonster GetMonster(int index)
    {
        return (index >= 0 && index < monsterList.Count) ? monsterList[index] : null;
    }

    // 키값으로 sprite 가져오기
    public Sprite GetIcon(string monsterName)
    {
        TestMonster monster = monsterList.Find(monster => monster.name == monsterName);
        return monster != null ? monster.icon : null;
    }
}
