public class OverworkSkill : QueenActiveSkillBase
{
    QueenCondition condition;
    float returnToValue;

    public override void Init()
    {
        base.Init();

        //info 초기화

        condition = GameManager.Instance.queen.condition;
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
