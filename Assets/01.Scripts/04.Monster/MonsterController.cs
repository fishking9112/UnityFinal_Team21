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
    private Action<GameObject> returnToPool;

    public void Init(Action<GameObject> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnSpawn() // GetObject 이후
    {

    }

    public void OnDespawn() // 실행하면 자동으로 반환
    {
        returnToPool?.Invoke(gameObject);
    }
    #endregion

    [Header("현재 데이터")]
    public MonsterInfo monsterInfo;

    public Transform target;
    public NavMeshAgent navMeshAgent;
    public SpriteRenderer sprite;
    public MonsterStateMachine stateMachine;

    // public MonsterFSM fsm;
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


        if (stateMachine == null)
            stateMachine = new(this);
        stateMachine.ChangeState(stateMachine.FindToDo); // 할 일 찾기

        MonsterManager.Instance.monsters.Add(gameObject, this);
        MonsterManager.Instance.idByMonsters[this.monsterInfo.id].Add(this);
    }

    protected override void TakeDamaged(float damage, float knockback = 0f)
    {
        base.TakeDamaged(damage);
    }
    protected override void Die()
    {
        MonsterManager.Instance.monsters.Remove(gameObject);
        MonsterManager.Instance.idByMonsters[this.monsterInfo.id].Add(this);
        OnDespawn();
    }

    // 테스트 코드 주석처리
    // public void Update()
    // {
    //     if (Input.GetMouseButtonDown(1))
    //     {
    //         OnDespawn();
    //     }
    // }

}
