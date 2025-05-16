using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class HeroAbilityThunder : HeroAbilitySystem
{
    private Hero hero;

    private ObjectPoolManager objectPoolManager;
    private CancellationTokenSource tk;

    private ParticleSystem particle;

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
        particle = GetComponentInChildren<ParticleSystem>();
    }
    private void OnEnable()
    {
        Initialize((int)IDHeroAbility.THUNDER);
        tk = new CancellationTokenSource();

    }

    /// <summary>
    /// 어떤 능력인지 구현하는 곳
    /// 오브젝트풀로 구현해야함
    /// </summary>
    protected override void ActionAbility()
    {
        if (hero == null)
        {
            return;
        }

        target = hero.FindNearestTarget();
        Thunder(tk.Token).Forget();
    }

    private async UniTask Thunder(CancellationToken tk)
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


        float angle = UnityEngine.Random.Range(0f, 360f);
        float distance = UnityEngine.Random.Range(0f, pivot.x);

        Vector2 randomPos = center + new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * distance;


        var thunder = objectPoolManager.GetObject<ThunderEffect>("HeroThunder", hero.transform.position);
        thunder.SetData(damage,knockback,size.x,damage_Range);

        await UniTask.WaitUntil(() => !particle.IsAlive(true));
        thunder.OnDespawn();
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
