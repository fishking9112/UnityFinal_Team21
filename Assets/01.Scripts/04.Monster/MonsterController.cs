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

    public void OnSpawn()
    {
        MonsterManager.Instance.monsters.Add(gameObject, this);
    }

    public void OnDespawn()
    {
        MonsterManager.Instance.monsters.Remove(gameObject);
        returnToPool?.Invoke(gameObject);
    }
    #endregion

    [Header("현재 데이터")]
    public MonsterInfo monsterInfo;
    private NavMeshAgent navMesh;
    private GameObject target;




    /// <summary>
    /// 최초 생성 시 한번만 실행(참조해서 수치 자동 수정)
    /// </summary>
    /// <param name="monsterInfo">참조 할 수치 데이터</param>
    public void StatInit(MonsterInfo monsterInfo)
    {
        this.monsterInfo = monsterInfo;
        base.StatInit(this.monsterInfo);
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
