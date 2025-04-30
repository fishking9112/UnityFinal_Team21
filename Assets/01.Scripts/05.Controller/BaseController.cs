using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public abstract class BaseController : MonoBehaviour
{
    [Header("현재 데이터")]
    public BaseStatData statData;
    public LayerMask attackLayer; // 공격할 레이어 (적)
    public LayerMask obstacleLayer; // 감지할 레이어 (장애물)

    [Header("핸들러")]
    [SerializeField] protected HealthHandler healthHandler;

    [Header("버프")]
    public float buffMoveSpeed = 1f;
    public float buffAttackDamage = 1f;
    public float buffAttackSpeed = 1f;

    public Dictionary<int, List<Buff>> buffDic = new Dictionary<int, List<Buff>>();

    protected virtual void Start()
    {

    }

    /// <summary>
    /// 최초 생성 시 한번만 실행(참조해서 수치 자동 수정)
    /// </summary>
    /// <param name="statInfo">참조 할 수치 데이터</param>
    public void StatInit(BaseStatData statData, bool isHealthUI = false)
    {
        this.statData = statData;
        healthHandler.Init(statData.health);
        SetHealthUI(isHealthUI);
    }

    public void SetHealthUI(bool isHealthUI)
    {
        healthHandler.ActiveHealthUI(isHealthUI);
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
    /// 현재 체력 회복
    /// </summary>
    public void Heal(float amount)
    {
        healthHandler.Heal(amount);
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
        ClearAllBuff();
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


    // =================================================================================
    //                               버프 관련 코드
    // =================================================================================


    /// <summary>
    /// 버프로 인한 수치 조정
    /// </summary>
    public void AttackDamageBuff(float amount)
    {
        buffAttackDamage *= (1 + amount);
    }
    public void AttackSpeedBuff(float amount)
    {
        buffAttackSpeed *= (1 + amount);
    }
    public void MoveSpeedBuff(float amount)
    {
        buffMoveSpeed *= (1 + amount);
    }

    /// <summary>
    /// 버프가 끝날 때 수치 되돌리기
    /// </summary>
    public void EndAttackDamageBuff()
    {
        buffAttackDamage = 1f;
    }
    public void EndAttackSpeedBuff()
    {
        buffAttackSpeed = 1f;
    }
    public void EndMoveSpeedBuff()
    {
        buffMoveSpeed = 1f;
    }

    // 버프 추가
    public Buff AddBuff(int id, int level, CancellationTokenSource token)
    {
        var buff = new Buff(id, level, token);

        if (!buffDic.ContainsKey(id))
        {
            buffDic[id] = new List<Buff>();
        }

        buffDic[id].Add(buff);
        return buff;
    }

    // 버프 제거
    public void RemoveBuff(int id, bool cancel = false)
    {
        if (!buffDic.TryGetValue(id, out var buffList))
        {
            return;
        }

        foreach (var buff in buffList)
        {
            buff.token?.Cancel();
            buff.token?.Dispose();
        }

        if (cancel)
        {
            return;
        }

        buffList.Clear();
        buffDic.Remove(id);
    }

    // 모든 버프 제거
    public void ClearAllBuff()
    {
        foreach (var key in new List<int>(buffDic.Keys))
        {
            if (buffDic.TryGetValue(key, out var buffList))
            {
                foreach (var buff in buffList)
                {
                    if (buff != null && buff.particle != null)
                    {
                        buff.particle.OnDespawn();
                        buff.particle = null;
                    }
                }
            }
            RemoveBuff(key);
        }
        buffDic.Clear();
    }
}