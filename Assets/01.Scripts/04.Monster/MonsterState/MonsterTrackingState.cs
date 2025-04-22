using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class MonsterTrackingState : MonsterBaseState
{
    public MonsterTrackingState(MonsterStateMachine stateMachine) : base(stateMachine) { }
    Vector2 boxSize = Vector2.zero;
    private CancellationTokenSource ctsMoveTarget;
    private CancellationTokenSource ctsAllSearch;
    private float searchArea = 20f;
    private float searchTimer = 0f;


    public override void Enter()
    {
        base.Enter();
        ctsMoveTarget?.Cancel();
        ctsMoveTarget?.Dispose(); // 메모리 누수 방지
        ctsMoveTarget = new CancellationTokenSource();
        // 0.1 초마다 움직임
        MoveTargetAsync(ctsMoveTarget.Token).Forget();

        ctsAllSearch?.Cancel();
        ctsAllSearch?.Dispose(); // 메모리 누수 방지
        ctsAllSearch = new CancellationTokenSource();
        // 3 초마다 검사
        TargetAllSearchAsync(ctsAllSearch.Token).Forget();
    }
    public override void Exit()
    {
        base.Exit();
        searchTimer = 0f;
        ctsMoveTarget?.Cancel();
        ctsMoveTarget?.Dispose(); // 메모리 누수 방지
        ctsMoveTarget = null;

        ctsAllSearch?.Cancel();
        ctsAllSearch?.Dispose(); // 메모리 누수 방지
        ctsAllSearch = null;
    }

    private async UniTaskVoid TargetAllSearchAsync(CancellationToken token)
    {
        while (true)
        {
            token.ThrowIfCancellationRequested(); // 취소되면 예외 발생

            TargetAllSearch();

            // 3초 대기 Task
            var delayTask = UniTask.Delay(3000, cancellationToken: token);

            // 조건 감시 Task
            var earlyExitTask = UniTask.WaitUntil(() => target == null || !target.gameObject.activeSelf);

            // 먼저 끝나는 것 하나만 기다림(3초 기다리거나 target이 없거나)
            await UniTask.WhenAny(earlyExitTask, delayTask);
        }
    }

    private async UniTaskVoid MoveTargetAsync(CancellationToken token)
    {
        while (true)
        {
            token.ThrowIfCancellationRequested(); // 취소되면 예외 발생
            MoveTarget();
            await UniTask.Delay(100, cancellationToken: token); // 100ms 대기
        }
    }

    /// <summary>
    /// 타겟 찾고 해당 타겟으로 움직이는 함수 (0.1초마다 실행됨)
    /// </summary>
    public void MoveTarget()
    {
        // 타겟 찾기
        TargetAreaSearch();

        // 타겟이 없다면 전체적으로 찾을 수도 있음

        // 타겟이 꺼져있다면 null로
        if (target != null && !target.gameObject.activeSelf)
        {
            stateMachine.Controller.target = null;
        }

        // 타겟이 없다면 움직임 없음 (0.1초마다 반복되게 여기서 return)
        if (target == null)
        {
            return;
        }

        // 움직임-----
        spum.PlayAnimation(PlayerState.MOVE, 0);
        // 속도 0.1f 곱해서 너프시킴
        spum.SetMoveSpeed(stateMachine.Controller.monsterInfo.moveSpeed);
        navMeshAgent.speed = stateMachine.Controller.monsterInfo.moveSpeed;

        // 타겟과의 거리
        targetDistance = (target.position - navMeshAgent.transform.position).magnitude;

        // 타겟과의 거리가 적절해졌다면
        if (stateMachine.Controller.monsterInfo.attackRange >= targetDistance)
        {
            // 타겟과 나 사이에 장애물이 있다면 계속 움직이기
            if (IsObstacleBetween(navMeshAgent.transform.position, target.position))
            {
                navMeshAgent.SetDestination(target.position); // 이동 업데이트

                // 방향 바꾸기 0.04f 이상의 속도가 나올 때 움직여야 버버벅임 없음
                if (navMeshAgent.velocity.magnitude >= stateMachine.Controller.monsterInfo.moveSpeed * 0.04f)
                    pivot.localScale = new Vector3(0 <= navMeshAgent.velocity.x ? -1 : 1, pivot.localScale.y, pivot.localScale.z);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.Attack); // 공격!
            }
        }
        else // 아니면 계속 target 위치로 이동할 수 있도록 업데이트하여 추적
        {
            navMeshAgent.SetDestination(target.position); // 이동 업데이트

            // 방향 바꾸기 0.04f 이상의 속도가 나올 때 움직여야 버버벅임 없음
            if (navMeshAgent.velocity.magnitude >= stateMachine.Controller.monsterInfo.moveSpeed * 0.04f)
                pivot.localScale = new Vector3(0 <= navMeshAgent.velocity.x ? -1 : 1, pivot.localScale.y, pivot.localScale.z);
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

    /// <summary>
    /// 지역에서 타겟 찾기
    /// </summary>
    public void TargetAreaSearch()
    {
        float minDist = float.MaxValue;
        Vector2 origin = navMeshAgent.transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, searchArea, stateMachine.Controller.attackLayer);
        Utils.DrawOverlapCircle(origin, searchArea, Color.red, 0.1f);
        Collider2D nearHit = null;

        foreach (var hit in hits)
        {
            float dist = Vector2.Distance(origin, hit.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearHit = hit;
            }
        }

        if (nearHit != null)
        {
            stateMachine.Controller.target = nearHit.gameObject.transform;
        }
    }

    /// <summary>
    /// 전체 Hero에서 타겟 찾기
    /// </summary>
    public void TargetAllSearch()
    {
        float minDist = float.MaxValue;
        Vector2 origin = navMeshAgent.transform.position;
        GameObject nearHero = null;

        foreach (var hero in HeroManager.Instance.hero.Keys)
        {
            float dist = Vector2.Distance(origin, hero.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                nearHero = hero;
            }
        }

        if (nearHero != null)
        {
            stateMachine.Controller.target = nearHero.gameObject.transform;
        }
    }
}