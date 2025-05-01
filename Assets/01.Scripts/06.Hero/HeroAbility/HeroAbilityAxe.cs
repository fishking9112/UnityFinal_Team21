using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HeroAbilityAxe : HeroAbilitySystem
{
    private ObjectPoolManager objectPoolManager;
    private Hero hero;

    // Start is called before the first frame update
    public override void Initialize(int id)
    {
        base.Initialize(id);
    }

    private void Start()
    {
        hero = GetComponent<Hero>();
        objectPoolManager = ObjectPoolManager.Instance;
    }

    private void OnEnable()
    {
        Initialize((int)IDHeroAbility.AXE);
        
    }


    protected override void ActionAbility()
    {
        if (hero == null)
        {
            return;
        }

        target = hero.FindNearestTarget();
        ShootAxe().Forget();
    }

    private async UniTaskVoid ShootAxe()
    {
        float angle;

        if (target == null)
        {
            return;
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
        this.enabled = false;
        token?.Cancel();
        token?.Dispose();
    }

    public override void AbilityLevelUp()
    {
        base.AbilityLevelUp();
    }

    public override void SetAbilityLevel(int level)
    {
        base.SetAbilityLevel(level);
        token = new CancellationTokenSource();
    }
}
