
public class MonsterFindToDoState : MonsterBaseState
{
    public MonsterFindToDoState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        // stateMachine.Controller.AnimationHandler.SetState(ActionState.Idle);
    }

    public override void FixedUpdate()
    {
        if (target != null)
        {
            stateMachine.ChangeState(stateMachine.Tracking);
            return;
        }

        if (target == null)
        {
            // TODO : 적을 찾거나 퀘스트를 찾기
            if (MonsterManager.Instance.testTarget != null)
            {
                stateMachine.Controller.target = MonsterManager.Instance.testTarget.transform;
            }
        }
    }
}