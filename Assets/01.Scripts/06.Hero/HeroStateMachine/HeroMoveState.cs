using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HeroMoveState : HeroBaseState
{
    private bool isMove;
    private CancellationTokenSource deadToken;

    public HeroMoveState(HeroState state) : base(state)
    {
    }

    public override void Enter()
    {
        base.Enter();

        deadToken = new CancellationTokenSource();

        state.dir = state.GetDir();
        isMove = true;
        MoveAndSearch().Forget();

    }

    private async UniTaskVoid MoveAndSearch()
    {
        while (isMove)
        {
            MoveHero();
            Search();
            await UniTask.Yield();
        }
    }

    public override void Exit()
    {
        base.Exit();
        isMove = false;
        deadToken?.Cancel();
        deadToken = null;
    }

    private void MoveHero()
    {
        state.hero.transform.Translate(state.moveSpeed * Time.deltaTime * state.dir);
    }

    private void Search()
    {
        // Find Enemy that inside check area
        Collider2D col = Physics2D.OverlapCircle(state.hero.transform.position, 3);
        if (col == null)
        {
            return;
        }
        else
        {
            //state.ChangeState(state.attackState);
        }
    }
}
