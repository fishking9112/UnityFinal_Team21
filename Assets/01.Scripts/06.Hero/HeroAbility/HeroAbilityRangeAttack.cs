using UnityEngine;

public class HeroAbilityRangeAttack : HeroAbilitySystem
{
    private Hero hero;

    private float range;

    private LayerMask layer;
    protected override void Start()
    {
        heroAbilityInfo = DataManager.Instance.heroAbilityDic[101];

        base.Start();
        hero = GameManager.Instance.hero;
        delay = heroAbilityInfo.delay_Base;
        damage = heroAbilityInfo.damage_Base;
        range = 3;  // 임시 값
        layer = LayerMask.GetMask("Monster");
        AddAbility();
    }

    protected override void ActionAbility()
    {
        Collider2D[] rangedTarget = Physics2D.OverlapCircleAll(hero.transform.position, range, layer);

        foreach (Collider2D c in rangedTarget)
        {
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
