public class RewardExp : RewardBase
{
    protected override void GainReward()
    {
        condition.AdjustCurExpGauge(rewardAmount);
    }
}
