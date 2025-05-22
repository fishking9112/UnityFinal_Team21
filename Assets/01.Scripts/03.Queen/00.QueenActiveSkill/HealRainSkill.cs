using UnityEngine;

public class HealRainSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.HEAL_RAIN];
    }

    public override void UseSkill()
    {
        foreach (var monster in MonsterManager.Instance.monsters)
        {
            monster.Value.Heal(info.value);

            Vector3 targetScale = monster.Value.gameObject.transform.localScale;
            Vector3 particlePos = monster.Value.gameObject.transform.position + new Vector3(0, targetScale.y * 0.1f, 0);
            Vector3 particleScale = targetScale * 0.1f;

            ParticleObject particle = ParticleManager.Instance.SpawnParticle("Heal", particlePos, particleScale, Quaternion.identity, monster.Value.transform);
        }
    }
}
