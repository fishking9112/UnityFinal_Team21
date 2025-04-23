using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

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

    }

    public void StatInit(HeroStatusInfo stat)
    {
        DeadCheck().Forget();
        stateMachine.ChangeState(stateMachine.moveState);

        hero.Init();
        base.StatInit(stat);

        hero.ResetAbility();

        for(int i=0;i<stat.weapon.Length;i++)
        {
            hero.SetAbilityLevel(stat.weapon[i], stat.weaponLevel[i]);
        }
    }

    public override void TakeDamaged(float damage)
    {
        base.TakeDamaged(damage);
    }
    private async UniTaskVoid DeadCheck()
    {
        await UniTask.WaitUntil(() => healthHandler.IsDie());
        stateMachine.ChangeState(stateMachine.deadState);
        ResetObj();
    }


    private void ResetObj()
    {
        HeroPoolManager.Instance.ReturnObject(this);
    }
}
