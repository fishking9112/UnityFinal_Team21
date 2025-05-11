using UnityEngine;

public class ManaRecycleSkill : QueenActiveSkillBase
{
    QueenCondition condition;

    public override void Init()
    {
        base.Init();
        condition = GameManager.Instance.queen.condition;
        //info 초기화
    }

    public override void UseSkill()
    {
        condition.AdjustCurQueenActiveSkillGauge(-30f);
        condition.AdjustCurSummonGauge(50f);
    }
}
