using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class MonsterAttackState : MonsterBaseState
{
    public MonsterAttackState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    private CancellationTokenSource cts;

    private float attackTimer;

    public override void Enter()
    {
        base.Enter();
        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector2.zero;
        float animationSpeed = Mathf.Clamp(stat.attackSpeed.Value * stateMachine.Controller.attackAnimSpeed, 0.1f, float.MaxValue);
        spum.SetAttackSpeed(animationSpeed);

        cts?.Cancel();
        cts?.Dispose();
        cts = new CancellationTokenSource();

        stateMachine.Controller.AttackStrategy.Attack(stateMachine, cts);
    }

    public override void Exit()
    {
        base.Exit();
        attackTimer = 0.0f;
        cts?.Cancel();
        cts?.Dispose(); // 메모리 누수 방지
        cts = null;
    }

    public override void Update()
    {
        base.Update();

        // 타겟이 꺼져있다면 null로
        if (target != null && !target.gameObject.activeSelf)
        {
            stateMachine.Controller.target = null;
            stateMachine.ChangeState(stateMachine.Tracking);
            return;
        }

        // 공격 이후 애니메이션이 끝나거나 공격 딜레이를 기다림
        attackTimer += Time.deltaTime;
        if (attackTimer < (1f / stat.attackSpeed.Value)) return;

        if (target == null)
        {
            stateMachine.ChangeState(stateMachine.Tracking);
            return;
        }

        targetDistance = (target.position - navMeshAgent.transform.position).magnitude - (0.35f * target.transform.localScale.z);

        // 타겟과의 거리가 적절해졌다면
        if (stat.attackRange.Value >= targetDistance)
        {
            // 타겟과 나 사이에 장애물이 있다면 계속 움직이기
            if (stateMachine.Controller.stateMachine.Tracking.IsObstacleBetween(navMeshAgent.transform.position, target.position))
            {
                stateMachine.ChangeState(stateMachine.Tracking);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.Attack); // 공격!
            }
        }
        else // 아니면 계속 target 위치로 이동할 수 있도록 업데이트하여 추적
        {
            stateMachine.ChangeState(stateMachine.Tracking);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (target != null)
        {
            // 방향 바꾸기
            pivot.localScale = new Vector3(navMeshAgent.transform.position.x < target.position.x ? -1 : 1, pivot.localScale.y, pivot.localScale.z);
        }
    }
}