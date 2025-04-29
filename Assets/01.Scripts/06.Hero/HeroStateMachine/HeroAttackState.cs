using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HeroAttackState : HeroBaseState
{
    private GameObject enemy;
    private CancellationTokenSource token;
    private float detectedRange;

    public HeroAttackState(HeroState state) : base(state)
    {
    }

    public override void Enter()
    {
        base.Enter();
        token = new CancellationTokenSource();
        detectedRange = state.controller.statusInfo.detectedRange;
        state.dir = GetEnemyDir();
        Move(token.Token).Forget();
    }

    private async UniTask Move(CancellationToken tk)
    {
        while (!token.IsCancellationRequested)
        {
            while (enemy != null && enemy.activeSelf)
            {
                if (state.navMeshAgent.remainingDistance < state.navMeshAgent.stoppingDistance)
                {
                    state.navMeshAgent.ResetPath();
                    await UniTask.WaitUntil(() => enemy.activeSelf == false, PlayerLoopTiming.Update, tk);
                    GetEnemyDir();
                    break;
                }
                else
                {
                    state.navMeshAgent.SetDestination(state.dir);
                }
                await UniTask.Yield(tk, true);
            }
            await UniTask.Yield(tk, true);
        }
    }


    public override void Exit()
    {
        base.Exit();
        token?.Cancel();
        token?.Dispose();
        token = null;
        enemy = null;
    }

    private Vector2 GetEnemyDir()
    {
        Collider2D col = Physics2D.OverlapCircle(state.hero.transform.position, detectedRange, 1 << 7 | 1 << 13);
        if (col == null)
        {
            state.ChangeState(state.moveState);
            return state.GetDir();
        }
        else
        {
            enemy = col.gameObject;
            state.dir = col.transform.position;
            state.navMeshAgent.SetDestination(state.dir);

            return state.dir;
        }

    }

}
