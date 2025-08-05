using System;
using UnityEngine;

public class RewardGold : RewardBase
{
    public override void Init(Action<Component> returnAction)
    {
        base.Init(returnAction);

        type = RewardType.GOLD;
        SetMagnetTarget(RewardManager.Instance.goldTarget);
    }

    protected override void GainReward()
    {
        condition.AdjustGold(rewardAmount);
    }
}
