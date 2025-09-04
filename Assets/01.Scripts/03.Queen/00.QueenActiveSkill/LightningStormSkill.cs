using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using System;

public class LightingStormSkill : QueenActiveSkillBase
{
    ParticleObject skillParticle;

    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.LIGHTNING_STORM];
    }

    public override async UniTask UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;
        float tickCount = 10;

        // 스킬 범위 파티클
        skillParticle = ParticleManager.Instance.SpawnParticle("LightningStorm_Range", mousePos, Vector3.one, Quaternion.identity);

        try
        {
            // 라이트닝 효과 실행
            await LightningStormEffect(mousePos, info.size, tickCount, this.GetCancellationTokenOnDestroy());

            // 피해 처리
            for (int i = 0; i < tickCount; i++)
            {
                Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, info.size, info.target);

                foreach (var hit in hits)
                {
                    if (HeroManager.Instance.hero.TryGetValue(hit.gameObject, out var hero))
                    {
                        hero.TakeDamaged(info.value);
                    }
                }

                await UniTask.Delay(300, false, PlayerLoopTiming.Update, cancellationToken: this.GetCancellationTokenOnDestroy());
            }
        }
        catch (OperationCanceledException)
        {
            // 중간에 취소 됨.
        }
        finally
        {
            skillParticle?.OnDespawn();
        }
    }

    private async UniTask LightningStormEffect(Vector3 pos, float size, float count, CancellationToken token)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = pos + (Vector3)UnityEngine.Random.insideUnitCircle * size;
            ParticleManager.Instance.SpawnParticle("LightningStorm", randomPos, Vector3.one, Quaternion.identity);

            await UniTask.Delay(300, false, PlayerLoopTiming.Update, cancellationToken: token);
        }
    }

    protected override bool RangeCheck()
    {
        return true;
    }
}
