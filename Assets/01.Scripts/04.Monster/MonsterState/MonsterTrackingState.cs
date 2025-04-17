using UnityEngine;

public class MonsterTrackingState : MonsterBaseState
{
    public MonsterTrackingState(MonsterStateMachine stateMachine) : base(stateMachine) { }
    Vector2 boxSize = Vector2.zero;
    public override void Enter()
    {
        base.Enter();
        spum.PlayAnimation(PlayerState.MOVE, 0);
        //? LATE : 속도 0.1f 곱해서 너프시킴
        spum.SetMoveSpeed(stateMachine.Controller.monsterInfo.moveSpeed * 0.1f);
        navMeshAgent.speed = stateMachine.Controller.monsterInfo.moveSpeed * 0.1f;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        //? LATE : 타겟이 없다면 다시 적 찾기
        if (target == null)
        {
            stateMachine.ChangeState(stateMachine.FindToDo); // 찾기!
            return;
        }

        targetDistance = (target.position - navMeshAgent.transform.position).magnitude;

        // 타겟과의 거리가 적절해졌다면
        if (stateMachine.Controller.monsterInfo.attackRange >= targetDistance)
        {
            // 타겟과 나 사이에 장애물이 있다면 계속 움직이기
            if (IsObstacleBetween(navMeshAgent.transform.position, target.position))
            {
                navMeshAgent.SetDestination(target.position); // 이동 업데이트
            }
            else
            {
                stateMachine.ChangeState(stateMachine.Attack); // 공격!
            }
        }
        else // 아니면 계속 target 위치로 이동할 수 있도록 업데이트하여 추적
        {
            navMeshAgent.SetDestination(target.position); // 이동 업데이트

            // 방향 바꾸기
            if (navMeshAgent.velocity.magnitude >= stateMachine.Controller.monsterInfo.moveSpeed * 0.04f)
                pivot.localScale = new Vector3(0 <= navMeshAgent.velocity.x ? -1 : 1, pivot.localScale.y, pivot.localScale.z);
            // pivot.localScale = new Vector3(navMeshAgent.transform.position.x < target.position.x ? -1 : 1, pivot.localScale.y, pivot.localScale.z);
        }
    }

    /// <summary>
    /// 만약 타겟과 나 사이에 장애물이 있다면 계속 움직이기
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public bool IsObstacleBetween(Vector2 from, Vector2 to)
    {
        Vector2 direction = to - from;
        Vector2 center = (to + from) * 0.5f;
        float distance = direction.magnitude;

        boxSize.x = distance;
        boxSize.y = stateMachine.Controller.projectileSize.y;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Collider2D hit = Physics2D.OverlapBox(center, boxSize, angle, stateMachine.Controller.obstacleLayer);
        Utils.DrawBoxCast(center, boxSize, angle, Color.red, 0.1f);
        return hit != null;
    }
}