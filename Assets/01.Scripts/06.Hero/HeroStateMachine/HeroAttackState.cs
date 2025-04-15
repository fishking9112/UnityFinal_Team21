using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HeroAttackState : HeroBaseState
{
    private GameObject enemy;
    private CancellationTokenSource token;
    private CancellationTokenSource deadToken;

    public HeroAttackState(HeroState state) : base(state)
    {
    }

    public override void StateEnter()
    {
        base.StateEnter();
        token = new CancellationTokenSource();
        deadToken = new CancellationTokenSource();
        state.dir = GetEnemyDir();
        Move(token.Token).Forget();
    }

    private async UniTaskVoid Move(CancellationToken tk)
    {
        while (!token.IsCancellationRequested)
        {
            while (enemy != null && enemy.activeSelf)
            {
                state.hero.transform.Translate(state.moveSpeed * Time.deltaTime * state.dir);

                await UniTask.Yield(tk,true);
            }
            GetEnemyDir();
            await UniTask.Yield(tk, true);
        }
    }


    public override void StateExit()
    {
        base.StateExit();
        token?.Cancel();
        deadToken?.Cancel();
        deadToken = null;
        token = null;
        enemy = null;
    }

    private Vector2 GetEnemyDir()
    {
        Collider2D col = Physics2D.OverlapCircle(state.hero.transform.position, 3);
        if (col == null)
        {
            state.ChangeState(state.moveState);
            return state.GetDir();
        }
        else
        {
            enemy = col.gameObject;
            state.dir=col.transform.position-state.hero.transform.position;
            return state.dir;
        }

    }
}
