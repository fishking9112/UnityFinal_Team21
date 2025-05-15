using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HeroAbilityHold : HeroAbilitySystem
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
        Initialize((int)IDHeroAbility.HOLD);
        tk=new CancellationTokenSource();
    }

    protected override void ActionAbility()
    {
        if (hero == null)
        {
            return;
        }

        target = hero.FindNearestTarget();
        SetHolding(tk.Token).Forget();
    }


    private async UniTask SetHolding(CancellationToken tk)
    {
        if (hero == null || token == null)
        {
            return;
        }

        if (target == null)
        {
            return;
        }
        Vector2 center = hero.transform.position;
   

        for (int i = 0; i < count; i++)
        {
            float angle = UnityEngine.Random.Range(0f, 360f);
            float distance = UnityEngine.Random.Range(0f, pivot.x);

            Vector2 randomPos = center + new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * distance;

            var eff = ParticleManager.Instance.SpawnParticle("", randomPos, size);

            await UniTask.Delay(TimeSpan.FromSeconds(countDelay),cancellationToken:tk);
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
