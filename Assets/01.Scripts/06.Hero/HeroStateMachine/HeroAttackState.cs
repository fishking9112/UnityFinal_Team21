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

        // 상태 진입에 필요한 객체들이 유효한지 먼저 확인
        if (state == null || state.controller == null || state.controller.statusInfo == null || state.hero == null || state.navMeshAgent == null)
        {
            // 문제가 있으면, 안전하게 이전 상태나 기본 상태로 돌아감
            // 여기서는 다시 추적 상태로 보내는 것이 합리적으로 보임
            state?.ChangeState(state.moveState); 
            return;
        }

        token = new CancellationTokenSource();
        detectedRange = state.controller.statusInfo.detectedRange;
        if (state.hero == null) // Additional null check for safety
        {
            state.ChangeState(state.moveState);
            return;
        }
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

                if (state.navMeshAgent != null && state.navMeshAgent.isActiveAndEnabled && state.navMeshAgent.remainingDistance < state.navMeshAgent.stoppingDistance)
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
                    if (state.navMeshAgent != null && state.navMeshAgent.isActiveAndEnabled)
                    {
                        state.navMeshAgent.SetDestination(state.dir);
                    }
                    else
                    {
                        state.ChangeState(state.moveState);
                        return;
                    }
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
        if (state.hero == null)
        {
            return state.GetDir();
        }
        enemy = state.hero?.FindNearestTarget();

        if(enemy==null)
        {
            state.ChangeState(state.moveState);
            return state.GetDir();
        }
        else
        {
            if (enemy.transform == null)
            {
                state.ChangeState(state.moveState);
                return state.GetDir();
            }
            state.dir = enemy.transform.position;
            if (state.navMeshAgent != null && state.navMeshAgent.isActiveAndEnabled)
            {
                state.navMeshAgent.SetDestination(state.dir);
            }
            else
            {
                state.ChangeState(state.moveState);
                return state.GetDir();
            }
            return state.dir;
        }

    }

}