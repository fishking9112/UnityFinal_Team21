using Cysharp.Threading.Tasks;
using UnityEngine;
using System;

public class GravityBallSkill : QueenActiveSkillBase
{
    [SerializeField] private float pullPower = 5f;
    [SerializeField] private float pullTime = 0.01f;

    private ParticleObject skillParticle;

    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.GRAVITYBALL];
    }

    public override async UniTask UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;

        Vector3 scale = new Vector3(info.size / 4, info.size / 4, 1f);
        skillParticle = ParticleManager.Instance.SpawnParticle("GravityBall", mousePos, scale, Quaternion.identity);

        float time = 0f;

        try
        {
            while (time < info.value)
            {
                PullHero(mousePos);
                await UniTask.Delay(
                    (int)(pullTime * 1000),
                    false,
                    PlayerLoopTiming.Update,
                    cancellationToken: this.GetCancellationTokenOnDestroy()
                );
                time += pullTime;
            }
        }
        catch (OperationCanceledException)
        {
            // 중간에 취소 됨
        }
        finally
        {
            // 반드시 파티클 정리
            if (skillParticle != null)
            {
                skillParticle.OnDespawn();
            }
        }
    }

    private void PullHero(Vector3 pos)
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, info.size, info.target);

        foreach(var hit in hits)
        {
            if(HeroManager.Instance.hero.TryGetValue(hit.gameObject,out var hero))
            {
                Vector3 dir = (pos - hero.transform.position).normalized;
                hero.transform.position += dir * pullPower * pullTime;
            }
        }
    }

    protected override bool RangeCheck()
    {
        return true;
    }
}
