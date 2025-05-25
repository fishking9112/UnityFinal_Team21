using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class HeroAbilityBurst : HeroAbilitySystem
{

    private Hero hero;

    private ObjectPoolManager objectPoolManager;


    private CancellationTokenSource tk;
    /// <summary>
    /// 선언과 동시에 호출하기. 값 입력
    /// </summary>
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
        Initialize((int)IDHeroAbility.BURST);
        tk = new CancellationTokenSource();
    }

    protected override void ActionAbility()
    {
        if (hero == null)
        {
            return;
        }

        target = hero.FindNearestTarget();
        ShootBullet(tk.Token).Forget();
    }


    private async UniTask ShootBullet(CancellationToken tk)
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

        for (int i = 0; i < count; i++)
        {
            if (hero == null || token.IsCancellationRequested)
            {
                return;
            }

            var bullet = objectPoolManager.GetObject<HeroBullet>("Bullet", hero.transform.position);
            bullet.SetBullet(duration, pierce, damage, speed, 0,size, knockback);
            bullet.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

            await UniTask.Delay(TimeSpan.FromSeconds(countDelay), false, PlayerLoopTiming.Update, cancellationToken: tk);
        }

    }

    public override void AbilityLevelUp()
    {
        base.AbilityLevelUp();
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
