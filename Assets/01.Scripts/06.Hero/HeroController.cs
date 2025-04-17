using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HeroController : BaseController , IPoolable
{
    #region IPoolable
    private Action<Component> returnToPool;

    public void Init(Action<Component> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnSpawn() // GetObject 이후
    {
        HeroManager.Instance.hero.Add(gameObject,this);
    }

    public void OnDespawn() // 실행하면 자동으로 반환
    {
        returnToPool?.Invoke(this);
        HeroManager.Instance.hero.Remove(gameObject);
    }
    #endregion

    public NavMeshAgent navMeshAgent;


    // Start is called before the first frame update
    protected override void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }




    public override void TakeDamaged(float damage)
    {
        base.TakeDamaged(damage);
    }

}
