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
        navMeshAgent.ResetPath();
        navMeshAgent.velocity = Vector2.zero;
        spum.PlayAnimation(PlayerState.ATTACK, 0);
        spum.SetAttackSpeed(stateMachine.Controller.monsterInfo.attackSpeed);

        // 원거리 공격은 projectile 생성
        if (stateMachine.Controller.monsterInfo.monsterAttackType == MonsterAttackType.RANGED)
        {
            RangedAttack();
            return;
        }

        // 근거리 공격
        if (stateMachine.Controller.monsterInfo.monsterAttackType == MonsterAttackType.MELEE)
        {
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
        if (attackTimer < (1f / stateMachine.Controller.monsterInfo.attackSpeed)) return;

        targetDistance = (target.position - navMeshAgent.transform.position).magnitude;

        // 타겟과의 거리가 적절해졌다면
        if (stateMachine.Controller.monsterInfo.attackRange >= targetDistance)
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

        // 1초 프레임에서 0.55때 공격됨
        UniTask.Delay((int)(550 * (1f / stateMachine.Controller.monsterInfo.attackSpeed)), cancellationToken: cts.Token).ContinueWith(() =>
        {
            float minDist = float.MaxValue;
            Vector2 origin = navMeshAgent.transform.position;
            Collider2D[] hits = Physics2D.OverlapCircleAll(origin, stateMachine.Controller.statData.attackRange, stateMachine.Controller.attackLayer);
            Utils.DrawOverlapCircle(origin, stateMachine.Controller.statData.attackRange, Color.red, 0.1f);
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
                    HeroManager.Instance.hero[nearHit.gameObject].TakeDamaged(stateMachine.Controller.statData.attack);
                    InGameUIManager.Instance.gameResult.resultDatas[stateMachine.Controller.monsterInfo.id].allDamage += stateMachine.Controller.statData.attack;
                }
                // nearHit.gameObject.GetComponent<BaseController>().TakeDamaged(stateMachine.Controller.statData.attack);
            }
        });
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
        UniTask.Delay((int)(650 * (1f / stateMachine.Controller.monsterInfo.attackSpeed)), cancellationToken: cts.Token).ContinueWith(() =>
        {
            var projectileObject = ObjectPoolManager.Instance.GetObject<ProjectileObject>(stateMachine.Controller.monsterInfo.projectile, navMeshAgent.transform.position);
            projectileObject.Set((target.position - navMeshAgent.transform.position).normalized, stateMachine.Controller);
        });
    }

}