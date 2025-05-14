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
        skillParticle = ParticleManager.Instance.SpawnParticle("Barrior", GameManager.Instance.castle.transform.position, new Vector3(0.7f, 0.7f, 0.7f));
        GameManager.Instance.castle.condition.SetInvincible(true);
        Invoke("EndSkill", info.value);
    }

    private void EndSkill()
    {
        GameManager.Instance.castle.condition.SetInvincible(false);
        skillParticle.OnDespawn();
    }
}
