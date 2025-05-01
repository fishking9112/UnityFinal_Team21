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
        float time = 0f;

        while (time < coolTime)
        {
            time += Time.deltaTime;

            if(index < coolTimeMask.Count)
            {
                coolTimeMask[index].fillAmount = 1f - (time / coolTime);
            }
            await UniTask.Yield();
        }
        if(index < coolTimeMask.Count)
        {
            coolTimeMask[index].fillAmount = 0f;
        }
    }
}
