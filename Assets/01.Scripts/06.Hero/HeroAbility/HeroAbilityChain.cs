using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HeroAbilityChain : HeroAbilitySystem
{
    private Hero hero;

    private List<GameObject> hitList=new List<GameObject>();

    private float range;

    private void Start()
    {
        hero = GetComponent<Hero>();
        range = 5;

    }

    public override void Initialize(int id)
    {
        base.Initialize(id);
    }

    private void OnEnable()
    {
        Initialize((int)IDHeroAbility.CHAIN);
    }

    protected override void ActionAbility()
    {
        target = hero.FindNearestTarget();
        Chaining().Forget();
    }

    private async UniTaskVoid Chaining()
    {
        hitList.Clear();

        for(int i=0;i<pierce;i++)
        {
            target= FindNextTarget();
            if (target == null)
            {
                break;
            }
            TakeDamaged();
            hitList.Add(target);

            await UniTask.Delay(TimeSpan.FromSeconds(countDelay));
        }
    }

    private GameObject FindNextTarget()
    {
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, range, 1 << 7 | 1 << 13);

        foreach (Collider2D c in col)
        {
            if(hitList.Contains(c.gameObject))
            {
                continue;
            }
            else
            {
                return c.gameObject;
            }
        }

        return null;
    }

    private void TakeDamaged()
    {
        if (MonsterManager.Instance.monsters.TryGetValue(target, out var monster))
        {
            monster.TakeDamaged(damage);
        }
    }

    public override void DespawnAbility()
    {
        this.enabled = false;
        token?.Cancel();
        token?.Dispose();
    }

    public override void SetAbilityLevel(int level)
    {
        base.SetAbilityLevel(level);
        token = new CancellationTokenSource();
    }
}
