using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class HeroMoveState : HeroBaseState
{
    private bool isMove;
    private CancellationTokenSource deadToken;

    public override void StateEnter()
    {
        base.StateEnter();

        // dir 찾기
        // dir방향 자동이동
        // 적 검사
        deadToken = new CancellationTokenSource();

        state.dir = state.GetDir();
        isMove = true;
        MoveAndSearch().Forget();
        DeadCheck(deadToken.Token).Forget();

    }

    private async UniTaskVoid MoveAndSearch()
    {
        while(isMove)
        {
            MoveHero();
            Search();
            await UniTask.Yield();
        }
    }

    public override void StateExit()
    {
        base.StateExit();
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
            state.ChangeState(state.attackState);
        }
    }
}
