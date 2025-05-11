using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HeroAbilityFireball : HeroAbilitySystem
{
    private Hero hero;
    private ObjectPoolManager objectPoolManager;


    public override void Initialize(int id)
    {
        base.Initialize(id);

    }


    private void Start()
    {
        hero = this.GetComponent<Hero>();
        objectPoolManager = ObjectPoolManager.Instance;

    }

    private void OnEnable()
    {
        Initialize((int)IDHeroAbility.TARGETTING);

    }

    protected override void ActionAbility()
    {
        if (hero == null)
        {
            return;
        }

        target = hero.FindNearestTarget();
        ShootFireball().Forget();
    }

    private async UniTask ShootFireball()
    {
        if (hero == null || token == null)
        {
            return;
        }

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

        var bullet = objectPoolManager.GetObject<HeroTargetBullet>("Fireball", hero.transform.position);
        bullet.SetBullet(damage, speed, knockback, target);
        bullet.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

    }

    public override void DespawnAbility()
    {
        token?.Cancel();
        token?.Dispose();
    }
    public override void SetAbilityLevel(int level)
    {
        base.SetAbilityLevel(level);
        token = new CancellationTokenSource();
    }

}
