using UnityEngine;

public class OverworkSkill : QueenActiveSkillBase
{
    float returnToValue;

    private ParticleObject skillParticle;

    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.OVERWORK];
    }
    public override void UseSkill()
    {
        Vector3 targetScale = GameManager.Instance.castle.transform.localScale;
        Vector3 particlePos = GameManager.Instance.castle.transform.position;
        Vector3 particleScale = targetScale * 1.5f;

        skillParticle = ParticleManager.Instance.SpawnParticle("Overwork", particlePos, particleScale);
        condition.AdjustCurSummonGauge(condition.MaxSummonGauge.Value);
        returnToValue = condition.SummonGaugeRecoverySpeed;
        condition.AdjustSummonGaugeRecoverySpeed(-returnToValue);

        Invoke("ReturnValue", 10f);
    }

    private void ReturnValue()
    {
        skillParticle.OnDespawn();
        condition.AdjustSummonGaugeRecoverySpeed(returnToValue);
    }
}
