using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueenActiveSkillSlot : BaseSlot<QueenActiveSkillBase>
{
    public List<Image> coolTimeMask;

    public override void AddSlot(int index, QueenActiveSkillBase skill)
    {
        base.AddSlot(index, skill);

        if (index < 0 || index >= slotIconList.Count)
        {
            return;
        }

        slotIconList[index].sprite = DataManager.Instance.iconAtlas.GetSprite(skill.info.icon);
        slotIconList[index].enabled = true;
        slotIconList[index].preserveAspect = true;

        if (index < coolTimeMask.Count)
        {
            coolTimeMask[index].fillAmount = 0f;
        }
    }

    public void AddSlotToEmpty(QueenActiveSkillBase skill)
    {
        for (int i = 0; i < 6; i++)
        {
            if (!slotDic.ContainsKey(i))
            {
                AddSlot(i, skill);
                return;
            }
        }

        Utils.Log("비어 있는 슬롯이 없습니다.");
    }

    public void StartCoolTimeUI(int index, float coolTime)
    {
        if (index < 0 || index >= coolTimeMask.Count)
        {
            return;
        }

        _ = ApplyCoolTimeUI(index, coolTime);
    }

    private async UniTaskVoid ApplyCoolTimeUI(int index, float coolTime)
    {
        var token = this.GetCancellationTokenOnDestroy();

        float time = 0f;

        while (time < coolTime)
        {
            time += Time.deltaTime;

            if(index < coolTimeMask.Count)
            {
                coolTimeMask[index].fillAmount = 1f - (time / coolTime);
            }
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
        if(index < coolTimeMask.Count)
        {
            coolTimeMask[index].fillAmount = 0f;
        }
    }
}
