using Cysharp.Threading.Tasks;
using Google.GData.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class HeroController : BaseController
{
    [SerializeField]private HeroState stateMachine;

    public NavMeshAgent navMeshAgent;
    [SerializeField]private Hero hero;

    public Transform pivot;

    private int currentDir;
    private int lastDir;

    private CancellationTokenSource token = new CancellationTokenSource();

    [SerializeField] public HeroStatusInfo statusInfo;

    public void InitHero()
    {
        stateMachine = new HeroState(hero,this);
        navMeshAgent = GetComponent<NavMeshAgent>();
        stateMachine.navMeshAgent = navMeshAgent;
        pivot = transform.GetChild(0);

        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        CheckFlip(token.Token).Forget();
    }



    public void StatInit(HeroStatusInfo stat)
    {
        hero.Init(stat.detectedRange);
        navMeshAgent.speed = stat.moveSpeed;
        DeadCheck().Forget();

        base.StatInit(stat);
        this.statusInfo.Copy(stat);

        hero.ResetAbility();

        for(int i=0;i<stat.weapon.Length;i++)
        {
            hero.SetAbilityLevel(stat.weapon[i], stat.weaponLevel[i]);
        }

        stateMachine.ChangeState(stateMachine.moveState);
        token = new CancellationTokenSource();
    }

    public override void TakeDamaged(float damage)
    {
        base.TakeDamaged(damage);
    }

    private async UniTaskVoid CheckFlip(CancellationToken tk)
    {
        lastDir = 0;
        while (!tk.IsCancellationRequested)
        {
            float x = navMeshAgent.desiredVelocity.x;

            currentDir = MathF.Sign(x);

            if (currentDir == 0)
            {
                currentDir = lastDir;
            }
            else if (currentDir != lastDir)
            {
                pivot.localScale = new Vector3(-currentDir, 1, 1);
                lastDir = currentDir;
            }

            await UniTask.Yield(tk);
        }
    }
    private async UniTaskVoid DeadCheck()
    {
        await UniTask.WaitUntil(() => healthHandler.IsDie(),cancellationToken:this.GetCancellationTokenOnDestroy());
        stateMachine.ChangeState(stateMachine.deadState);
        ResetObj();
    }


    private void ResetObj()
    {
        token?.Cancel();
        token?.Dispose();
        HeroPoolManager.Instance.ReturnObject(this);
    }
}
