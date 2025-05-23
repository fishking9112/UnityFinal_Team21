using UnityEngine;

public class HealWaveSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.HEAL_WAVE];
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

                Vector3 targetScale = monster.transform.localScale;
                Vector3 particlePos = monster.transform.position + new Vector3(0, targetScale.y * 0.1f, 0);
                Vector3 particleScale = targetScale * 0.1f;

                ParticleObject particle = ParticleManager.Instance.SpawnParticle("Heal", particlePos, particleScale, Quaternion.identity, monster.transform);
            }
        }
    }
}
