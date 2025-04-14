using UnityEngine;

public class MonsterTrackingState : MonsterBaseState
{
    public MonsterTrackingState(MonsterStateMachine stateMachine) : base(stateMachine) { }
    Vector2 boxSize = Vector2.zero;
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
            sprite.flipX = navMeshAgent.transform.position.x < target.position.x; // 방향 바꾸기
        }
    }

    public bool IsObstacleBetween(Vector2 from, Vector2 to)
    {
        Vector2 direction = to - from;
        Vector2 center = (to + from) * 0.5f;
        float distance = direction.magnitude;

        boxSize.x = distance;
        boxSize.y = stateMachine.Controller.projectileSize.y;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Collider2D hit = Physics2D.OverlapBox(center, boxSize, angle, stateMachine.Controller.obstacleLayer);
        DrawBoxCast(center, boxSize, angle, Color.red, 0.1f);

        return hit != null;
    }

    public void DrawBoxCast(Vector2 center, Vector2 size, float angleDeg, Color color, float duration = 0.1f)
    {
        // 회전값을 라디안으로 변환
        float angleRad = angleDeg * Mathf.Deg2Rad;
        Quaternion rotation = Quaternion.Euler(0, 0, angleDeg);

        // 박스의 4개 꼭짓점 구하기 (기준은 중심에서 오프셋)
        Vector2 halfSize = size * 0.5f;

        Vector2 topRight = new Vector2(halfSize.x, halfSize.y);
        Vector2 topLeft = new Vector2(-halfSize.x, halfSize.y);
        Vector2 bottomLeft = new Vector2(-halfSize.x, -halfSize.y);
        Vector2 bottomRight = new Vector2(halfSize.x, -halfSize.y);

        // 회전 적용 후 월드 좌표로 변환
        topRight = center + (Vector2)(rotation * topRight);
        topLeft = center + (Vector2)(rotation * topLeft);
        bottomLeft = center + (Vector2)(rotation * bottomLeft);
        bottomRight = center + (Vector2)(rotation * bottomRight);

        // 박스 테두리 그리기
        Debug.DrawLine(topRight, topLeft, color, duration);
        Debug.DrawLine(topLeft, bottomLeft, color, duration);
        Debug.DrawLine(bottomLeft, bottomRight, color, duration);
        Debug.DrawLine(bottomRight, topRight, color, duration);
    }
}