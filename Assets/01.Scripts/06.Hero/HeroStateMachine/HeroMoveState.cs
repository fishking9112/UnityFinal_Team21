using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class HeroMoveState : HeroBaseState
{
    private bool isMove;
    private CancellationTokenSource token;

    private float detectedRange;
    public HeroMoveState(HeroState state) : base(state)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        // 필수 컴포넌트가 없으면, 오류를 기록하고 히어로를 비활성화하여 게임이 멈추는 것을 방지
        if (state?.controller == null || state.hero == null || state.controller.statusInfo == null)
        {
            Utils.Log("HeroMoveState 진입 오류: 필수 컴포넌트가 없음, 히어로 비활성화");
            return;
        }

        token = new CancellationTokenSource();
        state.dir = state.GetDir();
        if (state.dir == Vector2.zero)
        {
            Utils.LogWarning($"dir이 0");
        }
        isMove = true;
        MoveAndSearch(token.Token).Forget();
        state.controller.SetMove(true);
        detectedRange = state.controller.statusInfo.detectedRange;
    }

    private async UniTask MoveAndSearch(CancellationToken tk)
    {
        MoveHero().Forget();
        while (isMove && !tk.IsCancellationRequested)
        {
            if (state.hero == null)
            {
                break;
            }
            navMeshAgent.speed = stat.moveSpeed.Value;

            Search().Forget();

            await UniTask.Yield(cancellationToken: tk);
        }
    }

    public override void Exit()
    {
        base.Exit();
        isMove = false;
        token?.Cancel();
        token?.Dispose();
    }

    private async UniTask MoveHero()
    {
        await UniTask.Yield();
        state.navMeshAgent.enabled = true;
        state.navMeshAgent.SetDestination(state.dir);
        //state.hero.transform.Translate(state.moveSpeed * Time.deltaTime * state.dir);
    }

    private async UniTask Search()
    {
        if (state.hero == null) return;

        // 확인 영역 내의 적 찾기
        Utils.DrawOverlapCircle(state.hero.transform.position, detectedRange, Color.red);
        Collider2D col = Physics2D.OverlapCircle(state.hero.transform.position, detectedRange, 1 << 7 | 1 << 13);
        if (col != null && col.gameObject != null)
        {
            if (state.attackState is HeroAttackState attackState)
            {
                attackState.TargetEnemy = col.gameObject;
            }
            await UniTask.NextFrame();
            state.ChangeState(state.attackState);
        }
    }
}