using UnityEngine;

public class MonsterAttackState : MonsterBaseState
{
    public MonsterAttackState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    private float attackTimer;

    public override void Enter()
    {
        base.Enter();
        // targetController.StatHandler.Damage(statHandler.AttackDamage);
        sprite.flipX = navMeshAgent.transform.position.x < target.position.x; // 방향 바꾸기
        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector2.zero;

        // 바로 공격!
        Utils.Log("공격!");
    }

    public override void Exit()
    {
        base.Exit();
        attackTimer = 0.0f;
    }


    public override void Update()
    {
        // 공격 이후 애니메이션이 끝나거나 공격 딜레이를 기다림
        attackTimer += Time.deltaTime;
        if (attackTimer < stateMachine.Controller.monsterInfo.attackSpeed) return;

        // 타겟이 없다면 다시 적 찾기
        if (target == null)
        {
            stateMachine.ChangeState(stateMachine.FindToDo); // 찾기!
            return;
        }

        // 타겟이 있으면 타겟과의 거리 확인
        targetDistance = (target.position - navMeshAgent.transform.position).magnitude;

        // 타겟과의 거리가 멀 경우 다시 추적
        if (stateMachine.Controller.monsterInfo.attackRange < targetDistance)
        {
            stateMachine.ChangeState(stateMachine.Tracking); // 거리가 멀면 다시 추적으로 변경
        }
        else // 아니면 다시 공격
        {
            stateMachine.ChangeState(stateMachine.Attack);
        }
    }
}