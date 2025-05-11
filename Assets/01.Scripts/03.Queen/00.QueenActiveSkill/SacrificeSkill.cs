using UnityEngine;

public class SacrificeSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        // info 초기화
    }
    public override void UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;
        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, info.size, info.target);

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
            ParticleObject skillParticle = ParticleManager.Instance.SpawnParticle("Sacrifice", monster.transform.position + new Vector3(0, 0.5f, 0), new Vector3(0.2f, 0.2f, 1f));

            monster.Die();
        }

        GameManager.Instance.queen.condition.AdjustCurSummonGauge(info.value);
    }
}
