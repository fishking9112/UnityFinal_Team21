using UnityEngine;
using UnityEngine.SocialPlatforms;

public class HeroAbilityRangeAttack : HeroAbilitySystem
{
    private Hero hero;

    private float range;

    private LayerMask layer;
    protected override void Start()
    {
        heroAbilityInfo = DataManager.Instance.heroAbilityDic[104];

        base.Start();
        hero = this.GetComponent<Hero>();
        range = heroAbilityInfo.size_Base.x;  // 임시 값
        layer = LayerMask.GetMask("Monster");
        AddAbility();
    }

    protected override void ActionAbility()
    {
        Collider2D[] rangedTarget = Physics2D.OverlapCircleAll(hero.transform.position, range, layer);
        Utils.DrawOverlapCircle(hero.transform.position, range, Color.red);

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

        // RangeAttack이 레벨업 시 증가해야 되는 스텟 증가 추가
    }

    public override void DespawnAbility()
    {

    }
}
