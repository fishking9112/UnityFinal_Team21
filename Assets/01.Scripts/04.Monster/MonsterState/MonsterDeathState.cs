using UnityEngine;

public class MonsterDeathState : MonsterBaseState
{
    public MonsterDeathState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        // stateMachine.Controller.AnimationHandler.SetState(ActionState.Idle);
        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector2.zero;
    }
}