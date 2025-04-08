using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAbilityMissile : HeroAbilitySystem
{
    private Vector2 fireDir=Vector2.left;

    private int bulletCount;
    private float damage;

    private Hero hero;

    [SerializeField] GameObject bullet;

    /// <summary>
    /// 선언과 동시에 호출하기. 값 입력
    /// </summary>
    public override void Init()
    {
        //hero=GameManager.Instance.hero;
        bulletCount = 1;
        damage = 5;
        AddAbility();
    }


    /// <summary>
    /// 능력 실제로 발동 하는 부분. 세팅해줄것 필요하면 해주기
    /// </summary>
    protected override void AddAbility()
    {
        base.AddAbility();
    }



    /// <summary>
    /// 어떤 능력인지 구현하는 곳
    /// </summary>
    protected override void ActionAbility()
    {
        Instantiate(bullet);
    }
}
