using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

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
    [SerializeField]private HeroState stateMachine;

    public NavMeshAgent navMeshAgent;
    [SerializeField]private Hero hero;


    public void InitHero()
    {
        stateMachine = new HeroState(hero);
        navMeshAgent = GetComponent<NavMeshAgent>();
        stateMachine.navMeshAgent = navMeshAgent;


        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }

    public void InitAbility(List<int> abList, List<int> abLev)
    {
        DeadCheck().Forget();
        stateMachine.ChangeState(stateMachine.moveState);

        hero.Init();
        hero.ResetAbility();

        for(int i=0;i<abList.Count;i++)
        {
            hero.SetAbilityLevel(abList[i], abLev[i]);
        }
    }


    public override void TakeDamaged(float damage)
    {
        base.TakeDamaged(damage);
    }
    private async UniTaskVoid DeadCheck()
    {
        // 사망 체크로 수정 핋요
        await UniTask.WaitUntil(() => gameObject.activeSelf == false);
        stateMachine.ChangeState(stateMachine.deadState);
    }
}
