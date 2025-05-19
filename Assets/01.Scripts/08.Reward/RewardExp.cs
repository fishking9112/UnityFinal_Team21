using System;
using UnityEngine;

public class RewardExp : RewardBase
{
    public override void Init(Action<Component> returnAction)
    {
        base.Init(returnAction);
        SetMagnetTarget(RewardManager.Instance.expTarget);
    }

    protected override void GainReward()
    {
        condition.AdjustCurExpGauge(rewardAmount);
    }
}
