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
        Vector3 targetScale = GameManager.Instance.castle.transform.localScale;
        Vector3 particlePos = GameManager.Instance.castle.transform.position;
        Vector3 particleScale = targetScale * 1.5f;

        ParticleManager.Instance.SpawnParticle("ManaRecycle", particlePos, particleScale);
        condition.AdjustCurSummonGauge(info.value);
    }

    protected override bool RangeCheck()
    {
        return true;
    }
}