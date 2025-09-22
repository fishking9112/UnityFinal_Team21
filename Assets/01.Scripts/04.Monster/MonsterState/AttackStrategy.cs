using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

// 전략 인터페이스
public interface IAttackStrategy
{
    void Attack(MonsterStateMachine stateMachine, CancellationTokenSource cts);
}

// 근거리 공격 전략
public class MeleeAttackStrategy : IAttackStrategy
{
    private readonly int _animationIndex;

    public MeleeAttackStrategy(int animationIndex)
    {
        _animationIndex = animationIndex;
    }

    public void Attack(MonsterStateMachine stateMachine, CancellationTokenSource cts)
    {
        stateMachine.Controller.spum.PlayAnimation(PlayerState.ATTACK, _animationIndex);
        MeleeAttackAsync(stateMachine, cts.Token).Forget();
    }

    private async UniTaskVoid MeleeAttackAsync(MonsterStateMachine stateMachine, CancellationToken token)
    {
        try
        {
            var stat = stateMachine.Controller.statHandler;
            float waitTime = 550f * (1f / (stat.attackSpeed.Value * stateMachine.Controller.attackAnimSpeed));
            await UniTask.Delay((int)waitTime, false, PlayerLoopTiming.Update, cancellationToken: token);

            if (token.IsCancellationRequested || stateMachine.Controller.navMeshAgent == null || stateMachine.Controller.target == null || stateMachine.Controller == null)
            {
                return;
            }

            float minDist = float.MaxValue;
            Vector2 origin = stateMachine.Controller.navMeshAgent.transform.position + ((stateMachine.Controller.target.transform.position - stateMachine.Controller.navMeshAgent.transform.position).normalized * stat.attackRange.Value / 2f);
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, stat.attackRange.Value, stateMachine.Controller.attackLayer);
            Utils.DrawOverlapCircle(origin, stat.attackRange.Value, Color.red, 0.1f);
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
                if (HeroManager.Instance.hero.ContainsKey(nearHit.gameObject))
                {
                    HeroManager.Instance.hero[nearHit.gameObject].TakeDamaged(stat.attack.Value);
                    StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().gameResultUI.resultDatas[stateMachine.Controller.monsterInfo.id].allDamage += stat.attack.Value;
                }
                else if (GameManager.Instance.miniBarracks.ContainsKey(nearHit.gameObject))
                {
                    GameManager.Instance.miniBarracks[nearHit.gameObject].TakeDamaged(stat.attack.Value);
                    StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().gameResultUI.resultDatas[stateMachine.Controller.monsterInfo.id].allDamage += stat.attack.Value;
                }
            }
        }
        catch (OperationCanceledException)
        {
            // Do nothing - 정상 취소
        }
    }
}

// 원거리 공격 전략
public class RangedAttackStrategy : IAttackStrategy
{
    private readonly int _animationIndex;

    public RangedAttackStrategy(int animationIndex)
    {
        _animationIndex = animationIndex;
    }

    public void Attack(MonsterStateMachine stateMachine, CancellationTokenSource cts)
    {
        stateMachine.Controller.spum.PlayAnimation(PlayerState.ATTACK, _animationIndex);
        RangedAttackAsync(stateMachine, cts.Token).Forget();
    }

    private async UniTaskVoid RangedAttackAsync(MonsterStateMachine stateMachine, CancellationToken token)
    {
        try
        {
            var stat = stateMachine.Controller.statHandler;
            float waitTime = 600f * (1f / (stat.attackSpeed.Value * stateMachine.Controller.attackAnimSpeed));
            await UniTask.Delay((int)waitTime, false, PlayerLoopTiming.Update, cancellationToken: token);

            if (token.IsCancellationRequested || stateMachine.Controller.navMeshAgent == null || stateMachine.Controller.target == null || stateMachine.Controller == null)
            {
                return;
            }

            var projectileObject = ObjectPoolManager.Instance.GetObject<MonsterProjectileObject>(stateMachine.Controller.monsterInfo.projectile, stateMachine.Controller.navMeshAgent.transform.position);
            projectileObject.Set((stateMachine.Controller.target.position - stateMachine.Controller.navMeshAgent.transform.position).normalized, stateMachine.Controller);
        }
        catch (OperationCanceledException)
        {
            // 정상 취소
        }
    }
}
