using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class MonsterKnockbackState : MonsterBaseState
{
    public MonsterKnockbackState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    private float knockbackTimer => stateMachine.Controller.knockbackDuration;
    private float knockbackPower => stateMachine.Controller.knockbackPower;
    private Vector2 knockbackDirection => stateMachine.Controller.knockback;
    private float duration = 0f;

    public override void Enter()
    {
        base.Enter();
        navMeshAgent.velocity = Vector2.zero;
        spum.PlayAnimation(PlayerState.DAMAGED, 0);
        spum.SetKnockbackSpeed(1 / knockbackTimer);

        navMeshAgent.speed = knockbackPower;

        Vector2 knockbackPosition = (Vector2)stateMachine.Controller.transform.position + knockbackDirection;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(knockbackPosition, out hit, 2.0f, NavMesh.AllAreas))
        {
            navMeshAgent.SetDestination(hit.position);
        }

        duration = 0.0f;
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void Update()
    {
        base.Update();

        // 넉백 딜레이를 기다림
        duration += Time.deltaTime;
        if (duration < knockbackTimer) return;

        stateMachine.ChangeState(stateMachine.Tracking);
    }
}