public class RewardGold : RewardBase
{
    protected override void GainReward()
    {
        condition.AdjustGold(rewardAmount);
    }
}
