using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MonsterDieState : MonsterBaseState
{
    public MonsterDieState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    private CancellationTokenSource cts;

    public override void Enter()
    {
        base.Enter();
        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector2.zero;
        // navMeshAgent.enabled = false;

        collider.enabled = false;

        spum.PlayAnimation(PlayerState.DEATH, 0);

        cts?.Cancel();
        cts?.Dispose(); // 메모리 누수 방지
        cts = new CancellationTokenSource();

        // 2초뒤 자원반납
        DieProccess().Forget();

        // TODO : 죽을 때 크리스탈 반환?
        RewardExp gainCristal = ObjectPoolManager.Instance.GetObject<RewardExp>("ExpReward", stateMachine.Controller.gameObject.transform.position);
    }

    public override void Exit()
    {
        base.Exit();
        cts?.Cancel();
        cts?.Dispose(); // 메모리 누수 방지
        cts = null;
    }
    private async UniTask DieProccess()
    {
        List<SpriteRenderer> renderers = stateMachine.Controller.renderers;
        float time = 2f;

        // 2초 동안 alpha 0로 FadeOut
        while (time > 0f)
        {
            time -= Time.deltaTime;

            float alpha = time / 2f;
            foreach (var renderer in renderers)
            {
                if (renderer == null)
                {
                    continue;
                }

                Color color = renderer.color;
                color.a = alpha;
                renderer.color = color;
            }

            await UniTask.Yield();
        }

        // 마지막에 확실히 0으로
        foreach (var renderer in renderers)
        {
            if (renderer == null)
            {
                continue;
            }

            Color color = renderer.color;
            color.a = 0f;
            renderer.color = color;
        }

        // 이후 ObjectPool에서 Release
        stateMachine.Controller.OnDespawn();
    }
}