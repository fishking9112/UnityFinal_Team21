using UnityEngine;

public class RewardExp : RewardBase
{
    protected override void GainReward()
    {
        condition.AdjustCurExpGauge(rewardAmount);
        Debug.Log(condition.CurExpGauge.Value);
    }
}
