using Cysharp.Threading.Tasks;
using System;
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
    public override async UniTask UseSkill()
    {
        Vector3 targetScale = GameManager.Instance.castle.transform.localScale;
        Vector3 particlePos = GameManager.Instance.castle.transform.position;
        Vector3 particleScale = targetScale * 1.5f;

        skillParticle = ParticleManager.Instance.SpawnParticle("Overwork", particlePos, particleScale);

        condition.AdjustCurSummonGauge(condition.MaxSummonGauge.Value);
        returnToValue = condition.SummonGaugeRecoverySpeed;
        condition.AdjustSummonGaugeRecoverySpeed(-returnToValue);

        try
        {
            // 스킬 지속 시간 대기
            await UniTask.Delay(
                TimeSpan.FromSeconds(info.value),
                false,
                PlayerLoopTiming.Update,
                cancellationToken: this.GetCancellationTokenOnDestroy()
            );
        }
        finally
        {
            // 스킬 종료 처리
            skillParticle?.OnDespawn();
            condition.AdjustSummonGaugeRecoverySpeed(returnToValue);
        }
    }

    protected override bool RangeCheck()
    {
        return true;
    }
}
