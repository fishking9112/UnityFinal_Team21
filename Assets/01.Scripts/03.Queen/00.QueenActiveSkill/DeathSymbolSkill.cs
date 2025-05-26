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

        if (hits.Length == 0)
        {
            return;
        }

        GameObject targetHero = null;
        float minDistance = float.MaxValue;

        foreach (var hit in hits)
        {
            if (hit == null || hit.gameObject == null)
            {
                continue;
            }

            float distance = Vector2.Distance(mousePos, hit.transform.position);

            if (distance < minDistance)
            {
                minDistance = distance;
                targetHero = hit.gameObject;
            }
        }

        if (targetHero == null)
        {
            return;
        }

        if (!HeroManager.Instance.hero.TryGetValue(targetHero, out var hero) || hero == null)
        {
            return;
        }
        if (hero.healthHandler.IsDie())
        {
            return;
        }


        await BuffManager.Instance.ApplyBuff(hero, info.buff_ID, info.buff_Level);

    }

    protected override bool RangeCheck()
    {
        Vector3 mousePos = controller.worldMousePos;
        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, info.size, info.target);
        return hits.Length > 0;
    }
}
