using Cysharp.Threading.Tasks;
using System;
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

    private ObjectPoolManager objectPoolManager;

    /// <summary>
    /// 선언과 동시에 호출하기. 값 입력
    /// </summary>
    public void Start()
    {
        hero=GameManager.Instance.hero;
        bulletCount = 2;
        damage = 5;
        objectPoolManager = ObjectPoolManager.Instance;
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
        target = hero.FindNearestTarget();
        ShootBullet().Forget();
    }

    private async UniTaskVoid ShootBullet()
    {
        float angle;

        if (target==null)
        {
            angle = 0;
        }
        else
        {
            angle = Mathf.Atan2(target.transform.position.y - hero.transform.position.y,
                target.transform.position.x - hero.transform.position.x) * Mathf.Rad2Deg;
        }

        for (int i = 0; i < bulletCount; i++)
        {
            var bullet = objectPoolManager.GetObject("bullet", hero.transform.position);
            bullet.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
        }
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
