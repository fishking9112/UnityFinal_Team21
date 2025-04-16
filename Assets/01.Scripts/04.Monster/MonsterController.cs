using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 외형은 Prefab으로 미리 등록해서 사용
/// </summary>
public class MonsterController : BaseController, IPoolable
{
    #region IPoolable
    private Action<Component> returnToPool;

    public void Init(Action<Component> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnSpawn() // GetObject 이후
    {

    }

    public void OnDespawn() // 실행하면 자동으로 반환
    {
        returnToPool?.Invoke(this);
    }
    #endregion

    [Header("현재 데이터")]
    public MonsterInfo monsterInfo;

    public Transform target;
    public NavMeshAgent navMeshAgent;
    public SpriteRenderer sprite;
    public MonsterStateMachine stateMachine;
    public Vector2 projectileSize = Vector2.zero;

    private void Update()
    {
        stateMachine.Update();
    }
    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    /// <summary>
    /// 최초 생성 시 한번만 실행(참조해서 수치 자동 수정)
    /// </summary>
    /// <param name="monsterInfo">참조 할 수치 데이터</param>
    public void StatInit(MonsterInfo monsterInfo)
    {
        this.monsterInfo = monsterInfo;
        base.StatInit(this.monsterInfo);

        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        if (sprite == null)
            sprite = GetComponent<SpriteRenderer>();

        // projectileObject의 XY값 가져오기
        if (monsterInfo.projectile != "" && projectileSize == Vector2.zero)
        {
            //? LATE : GetComponent..!
            var projectileObject = ObjectPoolManager.Instance.GetObject<ProjectileObject>(monsterInfo.projectile, transform.position);
            //var projectileObject = go.GetComponent<ProjectileObject>();
            projectileSize = projectileObject._boxCollider.size;
            projectileObject.OnDespawn();
        }

        if (stateMachine == null)
            stateMachine = new(this);
        stateMachine.ChangeState(stateMachine.FindToDo); // 할 일 찾기

        MonsterManager.Instance.monsters.Add(gameObject, this);
        MonsterManager.Instance.idByMonsters[this.monsterInfo.id].Add(this);
    }

    /// <summary>
    /// 데미지를 입었을 경우
    /// </summary>
    /// <param name="damage"></param>
    public override void TakeDamaged(float damage)
    {
        base.TakeDamaged(damage);
    }

    /// <summary>
    /// 사망했을 경우
    /// </summary>
    protected override void Die()
    {
        MonsterManager.Instance.monsters.Remove(gameObject);
        MonsterManager.Instance.idByMonsters[this.monsterInfo.id].Remove(this);

        stateMachine.ChangeState(stateMachine.Die); // 사망
        OnDespawn();
    }
}
