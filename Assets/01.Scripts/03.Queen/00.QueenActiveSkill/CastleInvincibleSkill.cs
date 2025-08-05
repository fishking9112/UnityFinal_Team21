using System.Threading;
using UnityEngine;

public class CastleInvincibleSkill : QueenActiveSkillBase
{
    ParticleObject skillParticle;

    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.CASTLE_INVINCIBLE];
    }

    public override void UseSkill()
    {
        Vector3 targetScale = GameManager.Instance.castle.transform.localScale;
        Vector3 particlePos = GameManager.Instance.castle.transform.position;
        Vector3 particleScale = targetScale * 0.7f;

        skillParticle = ParticleManager.Instance.SpawnParticle("Barrior", particlePos, particleScale);
        GameManager.Instance.castle.condition.SetInvincible(true);
        Invoke("EndSkill", info.value);
    }

    private void EndSkill()
    {
        GameManager.Instance.castle.condition.SetInvincible(false);
        skillParticle.OnDespawn();
    }

    protected override bool RangeCheck()
    {
        return true;
    }
}
