using UnityEngine;

public class MonsterHealSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.RANGE_HEAL];
    }

    public override void UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;

        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, info.size, info.target);

        foreach (var hit in hits)
        {
            if (MonsterManager.Instance.monsters.TryGetValue(hit.gameObject, out var monster))
            {
                monster.Heal(info.value);
                ParticleObject particle = ParticleManager.Instance.SpawnParticle("Heal", monster.transform.position + new Vector3(0, 0.1f, 0), Quaternion.identity, 0.1f, monster.transform);
            }
        }
    }
}
