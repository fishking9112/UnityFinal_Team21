public class OverworkSkill : QueenActiveSkillBase
{
    float returnToValue;

    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.OVERWORK];
    }
    public override void UseSkill()
    {
        condition.AdjustCurSummonGauge(condition.MaxSummonGauge.Value);
        returnToValue = condition.SummonGaugeRecoverySpeed;
        condition.AdjustSummonGaugeRecoverySpeed(-returnToValue);

        Invoke("ReturnValue", 10f);
    }

    private void ReturnValue()
    {
        condition.AdjustSummonGaugeRecoverySpeed(returnToValue);
    }
}
