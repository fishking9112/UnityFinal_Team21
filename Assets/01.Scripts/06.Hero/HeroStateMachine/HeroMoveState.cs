using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class HeroMoveState : HeroBaseState
{
    private bool isMove;
    private CancellationTokenSource token;

    private float detectedRange;
    public HeroMoveState(HeroState state) : base(state)
    {
    }

    public override void Enter()
    {
        base.Enter();
        token=new CancellationTokenSource();
        state.dir = state.GetDir();
        isMove = true;
        MoveAndSearch(token.Token).Forget();
        detectedRange = state.controller.statusInfo.detectedRange;
    }

    private async UniTask MoveAndSearch(CancellationToken tk)
    {
            MoveHero();
        while (isMove)
        {
            Search(); 

            await UniTask.Yield(cancellationToken: tk);
        }
    }

    public override void Exit()
    {
        base.Exit();
        isMove = false;
        token?.Cancel();
        token?.Dispose();
    }

    private void MoveHero()
    {
        state.navMeshAgent.SetDestination(state.dir);
        //state.hero.transform.Translate(state.moveSpeed * Time.deltaTime * state.dir);
    }

    private void Search()
    {
        // Find Enemy that inside check area
        Utils.DrawOverlapCircle(state.hero.transform.position, detectedRange, Color.red);
        Collider2D col = Physics2D.OverlapCircle(state.hero.transform.position, detectedRange,1<<7|1<<13);
        if (col == null)
        {
            return;
        }
        else
        {
            state.ChangeState(state.attackState);
        }
    }

}
