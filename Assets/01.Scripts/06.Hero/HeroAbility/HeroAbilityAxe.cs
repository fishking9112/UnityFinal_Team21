using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAbilityAxe : HeroAbilitySystem
{
    private float pierce;
    private float duration;
    private int count;
    private float delayCount;
    private float rotateSpeed;
    private float speed;

    private LayerMask layer;
    private ObjectPoolManager objectPoolManager;
    private Hero hero;

    // Start is called before the first frame update
    protected override void Start()
    {
        heroAbilityInfo = DataManager.Instance.heroAbilityDic[105];

        base.Start();

        pierce = heroAbilityInfo.piercing_Base;
        duration = heroAbilityInfo.duration_Base;
        count = heroAbilityInfo.count_Base;
        delayCount = heroAbilityInfo.countDelay_Base;
        rotateSpeed = heroAbilityInfo.rotateSpeed_Base;
        speed = heroAbilityInfo.speed_Base;

        layer = LayerMask.GetMask("Monster");

        hero = GetComponent<Hero>();
        objectPoolManager = ObjectPoolManager.Instance;

        AddAbility();
    }

    protected override void ActionAbility()
    {
        target = hero.FindNearestTarget();
        ShootAxe().Forget();
    }

    private async UniTaskVoid ShootAxe()
    {
        float angle;

        if (target == null)
        {
            angle = 0;
        }
        else
        {
            angle = Mathf.Atan2(target.transform.position.y - hero.transform.position.y,
                target.transform.position.x - hero.transform.position.x) * Mathf.Rad2Deg;
        }

        for (int i = 0; i < count; i++)
        {
            var bullet = objectPoolManager.GetObject<HeroBullet>("Axe", hero.transform.position);
            bullet.SetBullet(duration, pierce, damage, speed, rotateSpeed);
            bullet.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

            await UniTask.Delay(TimeSpan.FromSeconds(delay));
        }
    }


    public override void DespawnAbility()
    {

    }

}
