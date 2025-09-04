using Cysharp.Threading.Tasks;
using UnityEngine;

public class CastleInvincibleSkill : QueenActiveSkillBase
{
    ParticleObject skillParticle;

    public override void Init()
    {
        base.Init();
        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.CASTLE_INVINCIBLE];
    }

    public override async UniTask UseSkill()
    {
        Vector3 targetScale = GameManager.Instance.castle.transform.localScale;
        Vector3 particlePos = GameManager.Instance.castle.transform.position;
        Vector3 particleScale = targetScale * 0.7f;

        skillParticle = ParticleManager.Instance.SpawnParticle("Barrior", particlePos, particleScale);
        GameManager.Instance.castle.condition.SetInvincible(true);

        try
        {
            // 스킬 지속 시간만큼 기다리기
            await UniTask.Delay(
                (int)(info.value * 1000),
                false,
                PlayerLoopTiming.Update,
                cancellationToken: this.GetCancellationTokenOnDestroy()
            );
        }
        finally
        {
            // 무조건 정리
            EndSkill();
        }
    }

    private void EndSkill()
    {
        GameManager.Instance.castle.condition.SetInvincible(false);
        skillParticle?.OnDespawn();
        skillParticle = null;
    }

    protected override bool RangeCheck()
    {
        return true;
    }
}
