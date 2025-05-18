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
        skillParticle = ParticleManager.Instance.SpawnParticle("Overwork", GameManager.Instance.castle.transform.position, new Vector3(1.5f, 1.5f, 1.5f));
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
