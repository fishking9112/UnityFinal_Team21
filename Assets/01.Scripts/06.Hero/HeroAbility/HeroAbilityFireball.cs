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

        ShootFireball();
    }

    private void ShootFireball()
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
            //angle = Mathf.Atan2(target.transform.position.y - hero.transform.position.y,
            //    target.transform.position.x - hero.transform.position.x) * Mathf.Rad2Deg;
            angle = UnityEngine.Random.Range(0f, 360f);
        }

        Vector2 nor= new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        Vector2 targetPos = (Vector2)hero.transform.position + nor * 3f;
        var bullet = objectPoolManager.GetObject<HeroTargetBullet>("HeroFireball", hero.transform.position);
        bullet.SetBullet(damage, speed, knockback, targetPos,damage_Range);
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
