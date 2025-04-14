using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAbilityRangeAttack : HeroAbilitySystem
{
    private float damage;


    private Hero hero;

    private float range;

    private LayerMask layer;
    public void Init()
    {
        hero = GameManager.Instance.hero;
        delayTime = 5;
        damage = 20;
        range = 3;
        layer= LayerMask.GetMask("Monster");
        AddAbility();
    }

    protected override void ActionAbility()
    {
        Collider2D[] rangedTarget = Physics2D.OverlapCircleAll(hero.transform.position, range, layer);

        foreach(Collider2D c in rangedTarget)
        {
            // 딕셔너리로 GetComponent없이 대미지 입히기
        }
    }



    /// <summary>
    /// 임시로 만든 데이터
    /// </summary>
    /// <param name="nowLv"></param>
    public override void AbilityLevelUp(int nowLv)
    {
        switch(nowLv)
        {
            case 0: case 1: case 2:
                damage += 5;
                break;
                case 3: case 4:
                range += 0.5f;
                break;
        }

    }

}
