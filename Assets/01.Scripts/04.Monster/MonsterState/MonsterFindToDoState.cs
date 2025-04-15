
public class MonsterFindToDoState : MonsterBaseState
{
    public MonsterFindToDoState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void FixedUpdate()
    {
        //? LATE : null 다 없애고
        if (target != null)
        {
            stateMachine.ChangeState(stateMachine.Tracking);
            return;
        }

        if (target == null)
        {
            //? LATE : 적을 찾거나 퀘스트를 찾기
            if (MonsterManager.Instance.testTarget != null)
            {
                stateMachine.Controller.target = MonsterManager.Instance.testTarget.transform;
            }
        }
    }
}