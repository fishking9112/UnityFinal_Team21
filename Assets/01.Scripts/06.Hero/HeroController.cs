using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public class HeroController : BaseController
{
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

        healthHandler.Init(100);
    }

    public void InitAbility(List<int> abList, List<int> abLev)
    {
        healthHandler.Init(100);

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
        await UniTask.WaitUntil(() => healthHandler.IsDie());
        stateMachine.ChangeState(stateMachine.deadState);
        ResetObj();
    }


    private void ResetObj()
    {
        HeroPoolManager.Instance.ReturnObject(this);
    }
}
