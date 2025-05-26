using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class QueenActiveSkillSlot : BaseSlot<QueenActiveSkillBase>
{
    public List<Image> coolTimeMask;

    private Dictionary<int, CancellationTokenSource> coolTimeTokenDic = new Dictionary<int, CancellationTokenSource>();

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

        slotCostTextList[index].text = skill.info.cost.ToString();

        if (index < coolTimeMask.Count)
        {
            coolTimeMask[index].fillAmount = 0f;
        }

        if (coolTimeTokenDic.TryGetValue(index, out var existToken))
        {
            existToken.Cancel();
            existToken.Dispose();
        }

        coolTimeTokenDic[index] = new CancellationTokenSource();

        // 해당 슬롯에 스킬설명을 위한 스킬 정보 넣기
        var trigger = slotIconList[index].GetComponent<SkillDescriptionUITrigger>();
        if (trigger != null)
        {
            trigger.skill = skill;
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

    public bool HasEmptySkillSlot()
    {
        for (int i = 0; i < 6; i++)
        {
            if (!slotDic.ContainsKey(i))
            {
                return true;
            }
        }

        return false;
    }

    public int GetSkillIDbyIndex(int index)
    {
        if (slotDic.TryGetValue(index, out var skill) && skill != null)
        {
            return skill.info.ID;
        }

        return -1;
    }

    public void StartCoolTimeUI(int index, float coolTime)
    {
        if (index < 0 || index >= coolTimeMask.Count)
        {
            return;
        }

        if (coolTimeTokenDic.TryGetValue(index, out var existToken))
        {
            existToken.Cancel();
            existToken.Dispose();
        }

        CancellationTokenSource newToken = new CancellationTokenSource();
        coolTimeTokenDic[index] = newToken;

        _ = ApplyCoolTimeUI(index, coolTime, newToken.Token);
    }

    private async UniTaskVoid ApplyCoolTimeUI(int index, float coolTime, CancellationToken token)
    {
        float time = 0f;

        try
        {
            while (time < coolTime)
            {
                time += Time.deltaTime;

                if (index < coolTimeMask.Count)
                {
                    coolTimeMask[index].fillAmount = 1f - (time / coolTime);
                }
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        catch (OperationCanceledException)
        {
            // 쿨타임 도중 취소된 경우. 무시해도 됨
        }
        
        if (index < coolTimeMask.Count)
        {
            coolTimeMask[index].fillAmount = 0f;
        }
    }

    public override void RemoveSlot(int index)
    {
        base.RemoveSlot(index);

        if(coolTimeTokenDic.TryGetValue(index,out var token))
        {
            token.Cancel();
            token.Dispose();
            coolTimeTokenDic.Remove(index);
        }

        if (index >= 0 && index < coolTimeMask.Count)
        {
            coolTimeMask[index].fillAmount = 0f;
        }
    }
}
