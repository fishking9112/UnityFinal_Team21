using UnityEngine;

public class SacrificeSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.SACRIFICE];
    }
    public override void UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;
        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, info.size, info.target);

        if (hits.Length == 0)
        {
            return;
        }

        GameObject targetMonster = hits[0].gameObject;
        float minDistance = Vector2.Distance(mousePos, hits[0].transform.position);

        for (int i = 1; i < hits.Length; i++)
        {
            float distance = Vector2.Distance(mousePos, hits[i].transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                targetMonster = hits[i].gameObject;
            }
        }


        if (MonsterManager.Instance.monsters.TryGetValue(targetMonster, out var monster))
        {
            Vector3 targetScale = monster.transform.localScale;
            Vector3 particlePos = monster.transform.position + new Vector3(0, targetScale.y * 0.5f, 0);
            Vector3 particleScale = targetScale * 0.2f;

            ParticleObject skillParticle = ParticleManager.Instance.SpawnParticle("Sacrifice", particlePos, particleScale);

            monster.Die();
        }

        condition.AdjustCurSummonGauge(info.value);
    }
}
