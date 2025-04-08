using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HeroAbilityMissile : HeroAbilitySystem
{
    private Vector2 fireDir=Vector2.left;

    private int bulletCount;
    private float damage;

    private Hero hero;

    [SerializeField] HeroBullet bullet;


    /// <summary>
    /// 선언과 동시에 호출하기. 값 입력
    /// </summary>
    public void Init()
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
    /// 오브젝트풀로 구현해야함
    /// </summary>
    protected override void ActionAbility()
    {
        var a= Instantiate<HeroBullet>(bullet);
        a.Init(Vector2.left, 3, 1, 1);
    }


    /// <summary>
    /// 임시로 넣은 데이터
    /// </summary>
    /// <param name="nowLv"></param>
    public override void AbilityLevelUp(int nowLv)
    {
        switch(nowLv)
        {
            case 0: case 1: case 4:
                bulletCount++;
                break;
            case 2: case 3:
                damage += 3;
                break;
        }
    }

    /// <summary>
    /// 임시로 만든 함수
    /// </summary>
    /// <param name="cnt"></param>
    public void IncreaseBulletCount(int cnt)
    {
        bulletCount += cnt;
    }
}
