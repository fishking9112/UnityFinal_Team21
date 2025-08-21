using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterSlot : BaseSlot<MonsterInfo>
{
    [Header("스택 UI")]
    public List<TextMeshProUGUI> stackCountText; // 스택 숫자를 표시할 Text
    public List<Image> stackCoolTimeMask; // 쿨타임 진행률을 표시할 Image (Fill Amount 방식)
    public List<int> monsterIdList; // 몬스터 ID 리스트

    // Queen의 몬스터 슬롯에 정보 추가
    public override void AddSlot(int index, MonsterInfo monster)
    {
        base.AddSlot(index, monster);

        if (index < 0 || index >= slotIconList.Count)
        {
            return;
        }

        slotIconList[index].sprite = DataManager.Instance.iconAtlas.GetSprite(monster.outfit);
        slotIconList[index].enabled = true;
        slotIconList[index].preserveAspect = true;

        slotCostTextList[index].text = monster.cost.ToString();

        // 해당 슬롯에 몬스터 설명을 위한 스킬 정보 넣기
        var trigger = slotIconList[index].GetComponent<MonsterDescriptionTrigger>();
        if (trigger != null)
        {
            trigger.monster = monster;
        }

        monsterIdList[index] = monster.ID;
    }

    public override void RemoveSlot(int index)
    {
        base.RemoveSlot(index);

        if (index >= 0 && index < stackCoolTimeMask.Count)
        {
            stackCoolTimeMask[index].fillAmount = 0f;
            stackCountText[index].text = "";
            monsterIdList[index] = 0;
        }
    }

    // 매 프레임 UI를 업데이트하기 위해 Update 메서드 추가
    private void Update()
    {
        if (MonsterSummonManager.Instance != null && !MonsterSummonManager.Instance.isInitialized)
        {
            return;
        }

        for (int i = 0; i < monsterIdList.Count; i++)
        {
            if (monsterIdList[i] == 0)
            {
                continue;
            }
            int monsterId = monsterIdList[i];
            if (MonsterSummonManager.Instance.GetStackInfo(monsterId) is var stackInfo)
            {
                stackCoolTimeMask[i].fillAmount = MonsterSummonManager.Instance.GetStackPercent(monsterId);
                stackCountText[i].text = stackInfo.CurrentStacks.ToString() + "/" + stackInfo.MaxStacks.ToString();
            }
        }
    }
}