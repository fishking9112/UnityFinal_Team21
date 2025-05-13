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
            ParticleObject particle = ParticleManager.Instance.SpawnParticle("Heal", monster.Value.transform.position + new Vector3(0, 0.1f, 0), new Vector3(0.1f, 0.1f, 1f), Quaternion.identity, monster.Value.transform);
        }
    }
}
