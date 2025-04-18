using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HeroMoveState : HeroBaseState
{
    private bool isMove;

    public HeroMoveState(HeroState state) : base(state)
    {
    }

    public override void Enter()
    {
        base.Enter();

        state.dir = state.GetDir();
        isMove = true;
        MoveAndSearch().Forget();

    }

    private async UniTaskVoid MoveAndSearch()
    {
            MoveHero();
        while (isMove)
        {
            Search(); 

            await UniTask.Yield();
        }
    }

    public override void Exit()
    {
        base.Exit();
        isMove = false;
    }

    private void MoveHero()
    {
        state.navMeshAgent.SetDestination(state.dir);
        //state.hero.transform.Translate(state.moveSpeed * Time.deltaTime * state.dir);
    }

    private void Search()
    {
        // Find Enemy that inside check area
        Utils.DrawOverlapCircle(state.hero.transform.position, 3, Color.red);
        Collider2D col = Physics2D.OverlapCircle(state.hero.transform.position, 3,7);
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
