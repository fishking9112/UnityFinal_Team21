using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseController : MonoBehaviour
{
    [Header("현재 데이터")]
    public BaseStatData statData;
    public LayerMask attackLayer; // 공격할 레이어 (적)
    public LayerMask obstacleLayer; // 감지할 레이어 (장애물)

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
    /// (중요) 체력이 늘어나면 늘어난 만큼 최대 체력 수정할 수 있게 실행 할 것
    /// </summary>
    /// <param name="statInfo">참조 할 수치 데이터</param>
    public void HealthStatUpdate()
    {
        healthHandler.SetMaxPoint(statData.health);
    }

    /// <summary>
    /// 데미지를 입음
    /// </summary>
    /// <param name="damage">공격 들어온 데미지 수치</param>
    public virtual void TakeDamaged(float damage)
    {
        float finalDamage = damage;//Mathf.Max(0, damage - statData.defence);
        healthHandler.Damage(finalDamage);

        if (healthHandler.IsDie())
        {
            Die();
        }
    }

    /// <summary>
    /// 넉백을 입음
    /// </summary>
    /// <param name="damage">공격 들어온 데미지 수치</param>
    public virtual void TakeKnockback(Transform other, float power, float duration)
    {
        // TODO : 넉백 개산 있다면 해야함
        // knockbackDuration = duration;
        // knockback = -(other.position - transform.position).normalized * power;
    }

    /// <summary>
    /// 사망함
    /// </summary>
    protected virtual void Die()
    {
        // Destroy(this.gameObject);
    }
}
