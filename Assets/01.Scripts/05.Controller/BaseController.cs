using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseController : MonoBehaviour
{
    [Header("현재 데이터")]
    public BaseStatData statData;

    [Header("핸들러")]
    [SerializeField] protected HealthHandler healthHandler;

    protected virtual void Start()
    {

    }

    /// <summary>
    /// 최초 생성 시 한번만 실행(참조해서 수치 자동 수정)
    /// </summary>
    /// <param name="statInfo">참조 할 수치 데이터</param>
    public void StatInit(BaseStatData statData)
    {
        this.statData = statData;
        healthHandler.Init(statData.health);
    }

    /// <summary>
    /// 데미지를 입음
    /// </summary>
    /// <param name="damage">공격 들어온 데미지 수치</param>
    protected virtual void TakeDamaged(float damage)
    {
        float finalDamage = Mathf.Max(0, damage - statData.defence);
        healthHandler.Damage(finalDamage);

        if (healthHandler.IsDie())
        {
            Die();
        }
    }

    /// <summary>
    /// 사망함
    /// </summary>
    protected virtual void Die() { }

    protected virtual void DetectTarget() { }

    protected virtual void AttackTarget() { }
    protected virtual void Move() { }

}
