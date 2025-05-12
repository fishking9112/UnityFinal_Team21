using UnityEngine;

public class ManaRecycleSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        //info 초기화
    }

    public override void UseSkill()
    {
        condition.AdjustCurQueenActiveSkillGauge(-30f);
        condition.AdjustCurSummonGauge(50f);
    }
}