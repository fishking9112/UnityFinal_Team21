using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class HeroAttackState : HeroBaseState
{
    public GameObject TargetEnemy { get; set; }
    private GameObject _enemy;
    private CancellationTokenSource _cancellationTokenSource;

    public HeroAttackState(HeroState state) : base(state)
    {
    }

    public override void Enter()
    {
        base.Enter();

        // 예외 처리
        if (state?.controller == null || state.hero == null || state.navMeshAgent == null || !state.navMeshAgent.isOnNavMesh)
        {
            state.ChangeState(state.moveState);
            return;
        }

        // 적 설정
        _enemy = TargetEnemy;
        TargetEnemy = null; // 속성 초기화

        if (_enemy == null)
        {
            _enemy = state.hero.FindNearestTarget();
        }

        if (_enemy == null) // 여전히 적이 없으면 이동 상태로 돌아감
        {
            state.ChangeState(state.moveState);
            return;
        }

        // 공격 로직 시작
        _cancellationTokenSource = new CancellationTokenSource();
        AttackLoop(_cancellationTokenSource.Token).Forget();
    }

    private async UniTask AttackLoop(CancellationToken cancellationToken)
    {
        // 유효한 타겟이 있고 작업이 취소되지 않는 한 반복
        while (_enemy != null && _enemy.activeInHierarchy && !cancellationToken.IsCancellationRequested && state.hero != null)
        {
            // 적을 바라보게 함
            state.dir = _enemy.transform.position;

            // 공격 범위 내에 있는지 확인
            if (Vector2.Distance(state.hero.transform.position, _enemy.transform.position) <= state.navMeshAgent.stoppingDistance)
            {
                // 이동을 멈추고 공격
                state.navMeshAgent.ResetPath();
                state.controller.SetMove(false);
                
                // 적이 사라지거나 작업이 취소될 때까지 여기서 대기
                await UniTask.WaitUntil(() => _enemy == null || !_enemy.activeInHierarchy, cancellationToken: cancellationToken);
            }
            else
            {
                // 적을 향해 이동
                state.controller.SetMove(true);
                if (state.navMeshAgent.isOnNavMesh)
                {
                    state.navMeshAgent.SetDestination(_enemy.transform.position);
                }
            }

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        // 루프가 끝나면 적이 죽었거나 공격을 멈춰야 한다는 의미
        // 이동/탐색 상태로 돌아감
        if (!cancellationToken.IsCancellationRequested)
        {
            state.ChangeState(state.moveState);
        }
    }

    public override void Exit()
    {
        base.Exit();
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
        _enemy = null;
    }
}