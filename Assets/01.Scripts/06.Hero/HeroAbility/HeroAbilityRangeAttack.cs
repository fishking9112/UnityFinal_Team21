using System.Threading;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class HeroAbilityRangeAttack : HeroAbilitySystem
{
    private Hero hero;

    private LayerMask layer;

    [SerializeField] GameObject magicCircle;

    GameObject circle;
    public override void Initialize(int id)
    {
        base.Initialize(id);

        circle = Instantiate(magicCircle, this.transform);

        circle.transform.localScale = size;
    }

    private void Start()
    {
        hero = this.GetComponent<Hero>();
        layer = LayerMask.GetMask("Monster", "Castle");
    }
    private void OnEnable()
    {
        Initialize((int)IDHeroAbility.GARLIC);

    }
    protected override void ActionAbility()
    {
        Collider2D[] rangedTarget = Physics2D.OverlapCircleAll(hero.transform.position, size.x * hero.transform.localScale.x, layer);
        Utils.DrawOverlapCircle(hero.transform.position, size.x * hero.transform.localScale.x, Color.red);

        foreach (Collider2D c in rangedTarget)
        {
            if (MonsterManager.Instance.monsters.TryGetValue(c.gameObject, out var monster))
            {
                monster.TakeKnockback(hero.transform, knockback);
                monster.TakeDamaged(damage);
            }
            else if (GameManager.Instance.castle.gameObject == c.gameObject)
            {
                GameManager.Instance.castle.TakeDamaged(damage);
            }
            else if (GameManager.Instance.miniCastles.TryGetValue(c.gameObject, out var miniCastle))
            {
                miniCastle.TakeDamaged(damage);
            }
        }
    }


    public override void AbilityLevelUp()
    {
        base.AbilityLevelUp();
    }

    public override void DespawnAbility()
    {
        token?.Cancel();
        token?.Dispose();
        Destroy(circle);
    }
    public override void SetAbilityLevel(int level)
    {
        base.SetAbilityLevel(level);
    }
}
