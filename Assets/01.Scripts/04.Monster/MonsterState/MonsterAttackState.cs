using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class MonsterAttackState : MonsterBaseState
{
    public MonsterAttackState(MonsterStateMachine stateMachine) : base(stateMachine) { }

    private CancellationTokenSource cts;

    private float attackTimer;

    public override void Enter()
    {
        base.Enter();

        sprite.flipX = navMeshAgent.transform.position.x < target.position.x; // 방향 바꾸기
        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector2.zero;

        // 원거리 공격은 projectile 생성
        if (stateMachine.Controller.monsterInfo.projectile != "")
        {
            var go = ObjectPoolManager.Instance.GetObject(stateMachine.Controller.monsterInfo.projectile, navMeshAgent.transform.position);
            // TODO : GetComponent를 안쓰는 방법이 뭐가 있을까?
            var projectileObject = go.GetComponent<ProjectileObject>();
            projectileObject.Set((target.position - navMeshAgent.transform.position).normalized, stateMachine.Controller);
            return;
        }

        // 근접공격은 0.25초 뒤에 Overlap생성 후 공격
        cts?.Cancel();
        cts?.Dispose(); // 메모리 누수 방지
        cts = new CancellationTokenSource();

        // 0.25초 뒤에 원을 생성해서 
        UniTask.Delay(250, cancellationToken: cts.Token).ContinueWith(() =>
        {
            float minDist = float.MaxValue;
            Vector2 origin = navMeshAgent.transform.position;
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, stateMachine.Controller.statData.attackRange, stateMachine.Controller.attackLayer);
            DrawOverlapCircle(origin, stateMachine.Controller.statData.attackRange, Color.red, 0.1f);
            Collider2D nearHit = null;
            Debug.Log(hits);

            foreach (var hit in hits)
            {
                // TODO : 제일 가까운 적 한명만?
                float dist = Vector2.Distance(origin, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearHit = hit;
                }
            }

            if (nearHit != null)
            {
                nearHit.GetComponent<BaseController>().TakeDamaged(stateMachine.Controller.statData.attack);
            }
        });
    }

    public override void Exit()
    {
        base.Exit();
        attackTimer = 0.0f;
        cts?.Cancel();
        cts?.Dispose(); // 메모리 누수 방지
        cts = null;
    }


    public override void Update()
    {
        // 공격 이후 애니메이션이 끝나거나 공격 딜레이를 기다림
        attackTimer += Time.deltaTime;
        if (attackTimer < stateMachine.Controller.monsterInfo.attackSpeed) return;

        stateMachine.ChangeState(stateMachine.Tracking); // 다시 추적으로 변경
        // // 타겟이 있으면 타겟과의 거리 확인
        // targetDistance = (target.position - navMeshAgent.transform.position).magnitude;

        // // 타겟과의 거리가 멀 경우 다시 추적
        // if (stateMachine.Controller.monsterInfo.attackRange < targetDistance)
        // {
        //     stateMachine.ChangeState(stateMachine.Tracking); // 거리가 멀면 다시 추적으로 변경
        // }
        // else // 아니면 다시 공격
        // {
        //     stateMachine.ChangeState(stateMachine.Attack);
        // }
    }
    public void DrawOverlapCircle(Vector2 origin, float radius, Color color, float duration = 0.1f)
    {
        // 원을 그리기 위한 360도 각도
        int segments = 36; // 원을 그릴 때 사용할 점의 수
        float angleStep = 360f / segments;

        Vector2 previousPoint = origin + new Vector2(radius, 0); // 처음 점

        for (int i = 1; i <= segments; i++)
        {
            // 현재 각도를 구하고, 해당 각도로 벡터 계산
            float angle = angleStep * i;
            Vector2 newPoint = origin + new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, Mathf.Sin(angle * Mathf.Deg2Rad) * radius);

            // 원을 이루는 점을 그리기
            Debug.DrawLine(previousPoint, newPoint, color, duration);

            // 이전 점을 현재 점으로 갱신
            previousPoint = newPoint;
        }
    }
}