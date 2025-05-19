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
        ParticleManager.Instance.SpawnParticle("ManaRecycle", GameManager.Instance.castle.transform.position, new Vector3(1.5f, 1.5f, 1.5f));
        condition.AdjustCurQueenActiveSkillGauge(-30f);
        condition.AdjustCurSummonGauge(50f);
    }
}