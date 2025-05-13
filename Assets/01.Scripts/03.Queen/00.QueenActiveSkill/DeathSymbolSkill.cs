using UnityEngine;

public class DeathSybolSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.DEATH_SYMBOL];
    }

    public override async void UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;
        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, info.size, info.target);

        GameObject targetHero = hits[0].gameObject;
        float minDistance = Vector2.Distance(mousePos, hits[0].transform.position);

        for (int i = 1; i < hits.Length; i++)
        {
            float distance = Vector2.Distance(mousePos, hits[i].transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                targetHero = hits[i].gameObject;
            }
        }

        if (HeroManager.Instance.hero.TryGetValue(targetHero, out var hero))
        {
            await BuffManager.Instance.ApplyBuff(hero, info.buff_ID, info.buff_Level);
        }
    }
}
