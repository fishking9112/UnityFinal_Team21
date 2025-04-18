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
    private CancellationTokenSource token;

    // Start is called before the first frame update
    public override void Initialize(int id)
    {
        base.Initialize(id);
    }

    private void Start()
    {


        hero = GetComponent<Hero>();
        objectPoolManager = ObjectPoolManager.Instance;
        token = new CancellationTokenSource();
        AddAbility();
    }

    protected override void ActionAbility()
    {
        target = hero.FindNearestTarget();
        ShootAxe(token.Token).Forget();
    }

    private async UniTaskVoid ShootAxe(CancellationToken tk)
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

            await UniTask.Delay(TimeSpan.FromSeconds(delay), false, PlayerLoopTiming.Update, cancellationToken: tk);
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
    }
}
