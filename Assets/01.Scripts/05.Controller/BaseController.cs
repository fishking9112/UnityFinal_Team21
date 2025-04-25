using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;


public abstract class BaseController : MonoBehaviour
{
    [Header("현재 데이터")]
    public BaseStatData statData;
    public LayerMask attackLayer; // 공격할 레이어 (적)
    public LayerMask obstacleLayer; // 감지할 레이어 (장애물)

    [Header("핸들러")]
    [SerializeField] protected HealthHandler healthHandler;

    public Dictionary<int, int> buffDic = new Dictionary<int, int>();
    public Dictionary<int, CancellationTokenSource> buffTokenDic = new Dictionary<int, CancellationTokenSource>();

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
    protected void HealthStatUpdate()
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

    public void AddBuffToken(int id, CancellationTokenSource token)
    {
        if (buffTokenDic.TryGetValue(id, out var exist))
        {
            exist?.Cancel();
            exist?.Dispose();
            exist = null;
        }
        buffTokenDic[id] = token;
    }

    public void RemoveBuffToken(int id, bool cancel = false)
    {
        if (buffTokenDic.TryGetValue(id, out var exist))
        {
            exist?.Cancel();

            if (!cancel)
            {
                exist?.Dispose();
                exist = null;
                buffTokenDic.Remove(id);
                buffDic.Remove(id);
            }
        }
    }

    /// <summary>
    /// 업그레이드
    /// </summary>
    /// <param name="amount"></param>
    public virtual void UpgradeHealth(float amount)
    {

    }
    public virtual void UpgradeAttack(float amount)
    {

    }
    public virtual void UpgradeAttackSpeed(float amount)
    {

    }
    public virtual void UpgradeMoveSpeed(float amount)
    {

    }

    // 모든 버프 제거
    public void ClearAllBuff()
    {
        foreach (var pair in buffDic)
        {
            if (DataManager.Instance.buffDic.TryGetValue(pair.Key, out var buffInfo))
            {
                BuffManager.Instance.RemoveBuff(this, buffInfo);
            }
        }

        foreach (var token in buffTokenDic)
        {
            token.Value?.Cancel();
            token.Value?.Dispose();
        }

        buffDic.Clear();
        buffTokenDic.Clear();
    }
}
