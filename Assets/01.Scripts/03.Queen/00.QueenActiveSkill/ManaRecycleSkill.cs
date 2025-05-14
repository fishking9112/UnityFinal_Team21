using UnityEngine;

public class ManaRecycleSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.MANA_RECYCLE];
    }

    public override void UseSkill()
    {
        condition.AdjustCurQueenActiveSkillGauge(-30f);
        condition.AdjustCurSummonGauge(50f);
    }
}