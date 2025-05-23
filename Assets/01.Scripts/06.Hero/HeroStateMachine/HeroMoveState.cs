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
        token = new CancellationTokenSource();
        state.dir = state.GetDir();
        if (state.dir == Vector2.zero)
        {
            Debug.LogWarning($"dir이 0");
        }
        isMove = true;
        MoveAndSearch(token.Token).Forget();
        state.controller.SetMove(true);
        detectedRange = state.controller.statusInfo.detectedRange;

    }

    private async UniTask MoveAndSearch(CancellationToken tk)
    {
        MoveHero().Forget();
        while (isMove)
        {
            if(state.hero == null)
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
        // Find Enemy that inside check area
        Utils.DrawOverlapCircle(state.hero.transform.position, detectedRange, Color.red);
        Collider2D col = Physics2D.OverlapCircle(state.hero.transform.position, detectedRange, 1 << 7 | 1 << 13);
        if (col == null)
        {
            return;
        }
        else
        {
            await UniTask.NextFrame();
            state.ChangeState(state.attackState);
        }
    }

}
