public class MonsterTrackingState : MonsterBaseState
{
    public MonsterTrackingState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();

        // stateMachine.Controller.AnimationHandler.SetState(ActionState.Idle);
    }

    public override void FixedUpdate()
    {
        // 타겟이 없다면 다시 적 찾기
        if (target == null)
        {
            stateMachine.ChangeState(stateMachine.FindToDo); // 찾기!
            return;
        }

        targetDistance = (target.position - navMeshAgent.transform.position).magnitude;

        // 타겟과의 거리가 적절해졌다면
        if (stateMachine.Controller.monsterInfo.attackRange >= targetDistance)
        {
            stateMachine.ChangeState(stateMachine.Attack); // 공격!
        }
        else // 아니면 계속 target 위치로 이동할 수 있도록 업데이트하여 추적
        {
            navMeshAgent.SetDestination(target.position); // 이동 업데이트
            sprite.flipX = navMeshAgent.transform.position.x < target.position.x; // 방향 바꾸기
        }
    }
}