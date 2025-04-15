/// <summary>
/// 몬스터 상태 머신 (상태 추가될 때 마다 추가해줘야함)
/// </summary>
public class MonsterStateMachine : StateMachine
{
    public MonsterController Controller { get; private set; }

    public MonsterFindToDoState FindToDo { get; private set; }
    public MonsterTrackingState Tracking { get; private set; }
    public MonsterAttackState Attack { get; private set; }
    public MonsterDieState Die { get; private set; }

    public MonsterStateMachine(MonsterController controller)
    {
        Controller = controller;

        FindToDo = new(this);
        Tracking = new(this);
        Attack = new(this);
        Die = new(this);
    }
}