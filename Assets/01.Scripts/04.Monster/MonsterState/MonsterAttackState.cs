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
            RangedAttack();
            return;
        }

        // 근거리 공격
        MeleeAttack();
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

        stateMachine.ChangeState(stateMachine.FindToDo); // 다시 적 찾기로 변경
    }

    /// <summary>
    /// 근거리 공격
    /// </summary>
    private void MeleeAttack()
    {
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
            Utils.DrawOverlapCircle(origin, stateMachine.Controller.statData.attackRange, Color.red, 0.1f);
            Collider2D nearHit = null;

            foreach (var hit in hits)
            {
                //? LATE : 제일 가까운 적 한명만?
                float dist = Vector2.Distance(origin, hit.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearHit = hit;
                }
            }

            if (nearHit != null)
            {
                nearHit.gameObject.GetComponent<BaseController>().TakeDamaged(stateMachine.Controller.statData.attack);
            }
        });
    }

    /// <summary>
    /// 원거리 공격
    /// </summary>
    private void RangedAttack()
    {
        var projectileObject = ObjectPoolManager.Instance.GetObject<ProjectileObject>(stateMachine.Controller.monsterInfo.projectile, navMeshAgent.transform.position);
        //? LATE : GetComponent를 안쓰는 방법이 뭐가 있을까?
        //var projectileObject = go.GetComponent<ProjectileObject>();
        projectileObject.Set((target.position - navMeshAgent.transform.position).normalized, stateMachine.Controller);
    }

}