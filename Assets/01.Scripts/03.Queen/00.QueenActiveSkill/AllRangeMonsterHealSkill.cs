using UnityEngine;

public class AllRangeMonsterHealSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.ALL_HEAL];
    }

    public override void UseSkill()
    {
        foreach (var monster in MonsterManager.Instance.monsters)
        {
            monster.Value.Heal(info.value);
            ParticleObject particle = ParticleManager.Instance.SpawnParticle("Heal", monster.Value.transform.position + new Vector3(0, 0.1f, 0), Quaternion.identity, 0.1f, monster.Value.transform);
        }
    }
}
