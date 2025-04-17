using System.Threading;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class HeroAbilityRangeAttack : HeroAbilitySystem
{
    private Hero hero;

    private LayerMask layer;
    protected override void Start()
    {
        heroAbilityInfo = DataManager.Instance.heroAbilityDic[104];

        base.Start();
        hero = this.GetComponent<Hero>();
        layer = LayerMask.GetMask("Monster");

        AddAbility();
    }

    protected override void ActionAbility()
    {
        Collider2D[] rangedTarget = Physics2D.OverlapCircleAll(hero.transform.position, size.x, layer);
        Utils.DrawOverlapCircle(hero.transform.position, size.x, Color.red);

        foreach (Collider2D c in rangedTarget)
        {
            if (MonsterManager.Instance.monsters.TryGetValue(c.gameObject, out var monster))
            {
                Utils.Log("마늘공격");
                monster.TakeDamaged(damage);
            }
            // 딕셔너리로 GetComponent없이 대미지 입히기
        }
    }

    public override void AbilityLevelUp()
    {
        base.AbilityLevelUp();
    }

    public override void DespawnAbility()
    {

    }
    public override void SetAbilityLevel(int level)
    {
        base.SetAbilityLevel(level);
    }
}
