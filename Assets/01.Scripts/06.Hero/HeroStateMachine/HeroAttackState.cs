using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        while (!token.IsCancellationRequested && state.hero != null)
        {
            while (enemy != null && enemy.activeSelf && state.hero != null)
            {
                state.dir = GetEnemyDir();

                if (state.navMeshAgent.remainingDistance < state.navMeshAgent.stoppingDistance)
                {
                    state.controller.SetMove(false);
                    state.navMeshAgent.ResetPath();
                    await UniTask.WaitUntil(() => { return enemy == null || !enemy.activeInHierarchy; }, PlayerLoopTiming.Update, tk);

                    if (state.hero == null)
                    {
                        return;
                    }

                    state.controller.SetMove(true);
                    GetEnemyDir();
                    break;
                }
                else
                {
                    state.navMeshAgent.SetDestination(state.dir);
                }
                await UniTask.Yield(tk, true);
            }
            GetEnemyDir();
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
        enemy = state.hero.FindNearestTarget();

        if(enemy==null)
        {
            state.ChangeState(state.moveState);
            return state.GetDir();
        }
        else
        {
            state.dir = enemy.transform.position;
            state.navMeshAgent.SetDestination(state.dir);
            return state.dir;
        }

    }

}
