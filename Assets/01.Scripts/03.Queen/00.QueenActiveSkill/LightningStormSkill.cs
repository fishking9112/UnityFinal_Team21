using Cysharp.Threading.Tasks;
using UnityEngine;

public class LightingStormSkill : QueenActiveSkillBase
{
    ParticleObject skillParticle;

    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.LIGHTNING_STORM];
    }

    public override async void UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;
        float tickCount = 10;

        LightningStormEffect(mousePos, info.size, tickCount).Forget();

        skillParticle = ParticleManager.Instance.SpawnParticle("LightningStorm_Range", mousePos, Vector3.one, Quaternion.identity);
        Invoke("ParticleDespawn", 3f);

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

            await UniTask.Delay(300, false, PlayerLoopTiming.Update);
        }
    }

    private async UniTaskVoid LightningStormEffect(Vector3 pos, float size, float count)
    {
        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = pos + (Vector3)Random.insideUnitCircle * size;
            ParticleManager.Instance.SpawnParticle("LightningStorm", randomPos, Vector3.one, Quaternion.identity);

            await UniTask.Delay(300, false, PlayerLoopTiming.Update);
        }
    }

    private void ParticleDespawn()
    {
        skillParticle.OnDespawn();
    }

    protected override bool RangeCheck()
    {
        return true;
    }
}
