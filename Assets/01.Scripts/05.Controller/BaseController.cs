using System.Collections.Generic;
using System.Threading;
using UnityEngine;


public abstract class BaseController : MonoBehaviour
{
    [Header("현재 데이터")]
    public LayerMask attackLayer; // 공격할 레이어 (적)
    public LayerMask obstacleLayer; // 감지할 레이어 (장애물)

    [Header("핸들러")]
    [SerializeField] protected HealthHandler healthHandler;
    [SerializeField] public StatHandler statHandler;

    public BuffController buffController;

    // 스텟핸들러가 있었으면 좋겠습니다
    // public float buffMoveSpeed = 1f;
    // public float buffAttackDamage = 1f;
    // public float buffAttackSpeed = 1f;

    protected virtual void Start()
    {

    }

    /// <summary>
    /// 최초 생성 시 한번만 실행(참조해서 수치 자동 수정)
    /// </summary>
    /// <param name="statInfo">참조 할 수치 데이터</param>
    public void StatInit(BaseStatData statData, bool isHealthUI = false)
    {
        healthHandler.Init(statData.health);

        if (statData is MonsterInfo monsterStat)
        {
            statHandler.Init(monsterStat.health, monsterStat.moveSpeed, monsterStat.attack, monsterStat.attackRange, monsterStat.attackSpeed);
        }
        else
        {
            statHandler.Init(statData.health, statData.moveSpeed);
        }

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
        healthHandler.SetMaxPoint(statHandler.health.Value);
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
    /// 사망함
    /// </summary>
    protected virtual void Die()
    {
        // Destroy(this.gameObject);
        buffController.ClearAllBuff();
    }

    //-----------------------------------------------
    // 아래 코드들은 스텟핸들러에서 관리하면 좋겠습니다
    //-----------------------------------------------

    /// <summary>
    /// 버프로 인한 수치 조정
    /// </summary>
    public void AttackDamageBuff(float amount)
    {
        statHandler.attack.AddModifier(ModifierType.Multiply, 10, 1 + amount);
        // buffAttackDamage *= (1 + amount);
    }
    public void AttackSpeedBuff(float amount)
    {
        statHandler.attackSpeed.AddModifier(ModifierType.Multiply, 10, 1 + amount);
        // buffAttackSpeed *= (1 + amount);
    }
    public void MoveSpeedBuff(float amount)
    {
        statHandler.moveSpeed.AddModifier(ModifierType.Multiply, 10, 1 + amount);
        // buffMoveSpeed *= (1 + amount);
    }

    /// <summary>
    /// 버프가 끝날 때 수치 되돌리기
    /// </summary>
    public void EndAttackDamageBuff()
    {
        statHandler.attack.RemoveModifier(ModifierType.Multiply, 10);
        // buffAttackDamage = 1f;
    }
    public void EndAttackSpeedBuff()
    {
        statHandler.attackSpeed.RemoveModifier(ModifierType.Multiply, 10);
        // buffAttackSpeed = 1f;
    }
    public void EndMoveSpeedBuff()
    {
        statHandler.moveSpeed.RemoveModifier(ModifierType.Multiply, 10);
        // buffMoveSpeed = 1f;
    }
}