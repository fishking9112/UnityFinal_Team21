using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 외형은 Prefab으로 미리 등록해서 사용
/// </summary>
public class MonsterController : BaseController, IPoolable
{
    [Header("현재 데이터")]
    MonsterInfo data;

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

    // 테스트 코드 주석처리
    // public void Update()
    // {
    //     if (Input.GetMouseButtonDown(1))
    //     {
    //         OnDespawn();
    //     }
    // }

    /// <summary>
    /// 몬스터 스텟 Setting에서 모두 결정
    /// </summary>
    /// <param name="info">계산되어 받아 온 데이터</param>
    public void Setting(MonsterInfo info)
    {
        healthHandler.Init(info.health);
        data = info;
    }

    /// <summary>
    /// 몬스터 스텟 Setting으로 재조절(게임 진행하면서 몬스터 스텟이 바뀔 때)
    /// </summary>
    /// <param name="info">다시 계산되어 받아 온 데이터</param>
    public void ReSetting(MonsterInfo info)
    {
        healthHandler.SetMax(info.health); // 체력따로 설정
        data = info;
    }
}
