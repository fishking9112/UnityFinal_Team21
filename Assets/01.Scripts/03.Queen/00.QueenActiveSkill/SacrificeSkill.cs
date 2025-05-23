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

        GameObject targetMonster = null;
        float minDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit == null || hit.gameObject == null)
            {
                continue;
            }

            float distance = Vector2.Distance(mousePos, hit.transform.position);

            if(distance < minDistance)
            {
                minDistance = distance;
                targetMonster = hit.gameObject;
            }
        }

        if(targetMonster == null)
        {
            return;
        }

        if(!MonsterManager.Instance.monsters.TryGetValue(targetMonster,out var monster) || monster == null)
        {
            return;
        }

        if (monster.healthHandler.IsDie())
        {
            return;
        }


        Vector3 targetScale = monster.transform.localScale;
        Vector3 particlePos = monster.transform.position + new Vector3(0, targetScale.y * 0.5f, 0);
        Vector3 particleScale = targetScale * 0.2f;

        ParticleManager.Instance.SpawnParticle("Sacrifice", particlePos, particleScale);
        monster.Die();

        condition.AdjustCurSummonGauge(info.value);
    }
}
