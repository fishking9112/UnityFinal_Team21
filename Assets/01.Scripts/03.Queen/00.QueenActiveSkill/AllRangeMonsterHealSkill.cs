using UnityEngine;

public class AllRangeMonsterHealSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[207];
    }

    public override void UseSkill()
    {
        foreach(var monster in MonsterManager.Instance.monsters)
        {
            monster.Value.Heal(info.value);
            ParticleObject particle = ParticleManager.Instance.SpawnParticle("Heal", monster.Value.transform.position, Quaternion.identity, 0.5f, monster.Value.transform);
        }
    }
}
