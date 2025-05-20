using Cysharp.Threading.Tasks;
using System;
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
        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector2.zero;
        float animationSpeed = Mathf.Clamp(stat.attackSpeed.Value * stateMachine.Controller.attackAnimSpeed, 0.1f, float.MaxValue);
        spum.SetAttackSpeed(animationSpeed);


        var attackType = stateMachine.Controller.monsterInfo.monsterAttackType;

        switch (attackType)
        {
            case MonsterAttackType.MELEE:
                spum.PlayAnimation(PlayerState.ATTACK, 0);
                break;
            case MonsterAttackType.RANGED:
                spum.PlayAnimation(PlayerState.ATTACK, 2);
                break;
            case MonsterAttackType.MAGIC:
                spum.PlayAnimation(PlayerState.ATTACK, 4);
                break;
            case MonsterAttackType.SHURIKEN:
                spum.PlayAnimation(PlayerState.ATTACK, 0);
                break;
        }


        // 원거리 공격은 projectile 생성
        if (attackType == MonsterAttackType.RANGED || attackType == MonsterAttackType.MAGIC || attackType == MonsterAttackType.SHURIKEN)
        {
            RangedAttack();
            return;
        }

        // 근거리 공격
        if (attackType == MonsterAttackType.MELEE)
        {
            spum.PlayAnimation(PlayerState.ATTACK, 0);
            MeleeAttack();
            return;
        }
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
        base.Update();

        // 타겟이 꺼져있다면 null로
        if (target != null && !target.gameObject.activeSelf)
        {
            stateMachine.Controller.target = null;
            stateMachine.ChangeState(stateMachine.Tracking);
            return;
        }

        // 공격 이후 애니메이션이 끝나거나 공격 딜레이를 기다림
        attackTimer += Time.deltaTime;
        if (attackTimer < (1f / stat.attackSpeed.Value)) return;

        if (target == null)
        {
            stateMachine.ChangeState(stateMachine.Tracking);
            return;
        }

        targetDistance = (target.position - navMeshAgent.transform.position).magnitude - (0.45f * target.transform.localScale.z);

        // 타겟과의 거리가 적절해졌다면
        if (stat.attackRange.Value >= targetDistance)
        {
            // 타겟과 나 사이에 장애물이 있다면 계속 움직이기
            if (stateMachine.Controller.stateMachine.Tracking.IsObstacleBetween(navMeshAgent.transform.position, target.position))
            {
                stateMachine.ChangeState(stateMachine.Tracking);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.Attack); // 공격!
            }
        }
        else // 아니면 계속 target 위치로 이동할 수 있도록 업데이트하여 추적
        {
            stateMachine.ChangeState(stateMachine.Tracking);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (target != null)
        {
            // 방향 바꾸기
            pivot.localScale = new Vector3(navMeshAgent.transform.position.x < target.position.x ? -1 : 1, pivot.localScale.y, pivot.localScale.z);
        }
    }

    /// <summary>
    /// 근거리 공격
    /// </summary>
    private void MeleeAttack()
    {
        cts?.Cancel();
        cts?.Dispose(); // 메모리 누수 방지
        cts = new CancellationTokenSource();

        try
        {
            // 1초 프레임에서 0.55때 공격됨
            UniTask.Delay((int)(550 * (1f / (stat.attackSpeed.Value * stateMachine.Controller.attackAnimSpeed))), false, PlayerLoopTiming.Update, cts.Token).ContinueWith(() =>
            {
                float minDist = float.MaxValue;
                Vector2 origin = navMeshAgent.transform.position;
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
            });
        }
        catch (OperationCanceledException)
        {

        }
    }

    /// <summary>
    /// 원거리 공격
    /// </summary>
    private void RangedAttack()
    {
        cts?.Cancel();
        cts?.Dispose(); // 메모리 누수 방지
        cts = new CancellationTokenSource();

        // 1초 프레임에서 0.65때 발사
        UniTask.Delay((int)(600 * (1f / (stat.attackSpeed.Value * stateMachine.Controller.attackAnimSpeed))), false, PlayerLoopTiming.Update, cts.Token).ContinueWith(() =>
        {
            var projectileObject = ObjectPoolManager.Instance.GetObject<MonsterProjectileObject>(stateMachine.Controller.monsterInfo.projectile, navMeshAgent.transform.position);
            projectileObject.Set((target.position - navMeshAgent.transform.position).normalized, stateMachine.Controller);
        });
    }

}