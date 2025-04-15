using UnityEngine;

public class MonsterDieState : MonsterBaseState
{
    public MonsterDieState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector2.zero;
    }
}