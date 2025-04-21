using System.Threading;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class HeroAbilityRangeAttack : HeroAbilitySystem
{
    private Hero hero;

    private LayerMask layer;
    public override void Initialize(int id)
    {
        base.Initialize(id);
    }

    private void Start()
    {
        hero = this.GetComponent<Hero>();
        layer = LayerMask.GetMask("Monster");
    }

    protected override void ActionAbility()
    {
        Collider2D[] rangedTarget = Physics2D.OverlapCircleAll(hero.transform.position, size.x, layer);
        Utils.DrawOverlapCircle(hero.transform.position, size.x, Color.red);

        foreach (Collider2D c in rangedTarget)
        {
            if (MonsterManager.Instance.monsters.TryGetValue(c.gameObject, out var monster))
            {
                monster.TakeDamaged(damage);
            }
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
