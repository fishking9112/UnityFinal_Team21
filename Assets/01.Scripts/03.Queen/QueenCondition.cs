using UnityEngine;

public class QueenCondition : MonoBehaviour
{
    [Header("초기 설정")]
    public float initSummonGaugeRecoverySpeed = 10f;
    public float initQueenActiveSkillGaugeRecoverySpeed = 5f;
    public float initMaxQueenActiveSkillGauge = 100f;
    public float initMaxSummonGauge = 100f;
    public float initCurExpGauge = 0f;
    public float initMaxExpGauge = 100f;
    public float initEvolutionPoint = 0f;
    public float initLevel = 1f;
    public float initGold = 0f; 

    private float expGainMultiplierPercent = 0f;
    private float goldGainMultiplierPercent = 0f;

    public float SummonGaugeRecoverySpeed { get; private set; }
    public float QueenActiveSkillGaugeRecoverySpeed { get; private set; }
    public ReactiveProperty<float> CurQueenActiveSkillGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> MaxQueenActiveSkillGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> CurSummonGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> MaxSummonGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> CurExpGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> MaxExpGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> EvolutionPoint { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> Level { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> Gold { get; private set; } = new ReactiveProperty<float>();

    private float ExpGainMultiplier => 1f + (expGainMultiplierPercent * 0.01f);
    private float GoldGainMultiplier => 1f + (goldGainMultiplierPercent * 0.01f);

    private void Awake()
    {
        SummonGaugeRecoverySpeed = initSummonGaugeRecoverySpeed;
        QueenActiveSkillGaugeRecoverySpeed = initQueenActiveSkillGaugeRecoverySpeed;

        CurQueenActiveSkillGauge.Value = initMaxQueenActiveSkillGauge;
        MaxQueenActiveSkillGauge.Value = initMaxQueenActiveSkillGauge;
        CurSummonGauge.Value = initMaxSummonGauge;
        MaxSummonGauge.Value = initMaxSummonGauge;
        Level.Value = initLevel;
        CurExpGauge.Value = initCurExpGauge;
        MaxExpGauge.Value = initMaxExpGauge;
        EvolutionPoint.Value = initEvolutionPoint;
        Gold.Value = initGold;
    }

    /// <summary>
    /// 현재 액티브 스킬 게이지 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustCurQueenActiveSkillGauge(float amount)
    {
        CurQueenActiveSkillGauge.Value = AdjustValue(CurQueenActiveSkillGauge.Value, amount, MaxQueenActiveSkillGauge.Value);
    }

    /// <summary>
    /// 최대 액티브 스킬 게이지 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustMaxQueenActiveSkillGauge(float amount)
    {
        MaxQueenActiveSkillGauge.Value = AdjustValue(MaxQueenActiveSkillGauge.Value, amount, float.MaxValue);
        CurQueenActiveSkillGauge.Value = AdjustValue(CurQueenActiveSkillGauge.Value, amount, MaxQueenActiveSkillGauge.Value);
    }

    /// <summary>
    /// 현재 소환 게이지 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustCurSummonGauge(float amount)
    {
        CurSummonGauge.Value = AdjustValue(CurSummonGauge.Value, amount, MaxSummonGauge.Value);
    }

    /// <summary>
    /// 최대 소환 게이지 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustMaxSummonGauge(float amount)
    {
        MaxSummonGauge.Value = AdjustValue(MaxSummonGauge.Value, amount, float.MaxValue);
        CurSummonGauge.Value = AdjustValue(CurSummonGauge.Value, amount, MaxSummonGauge.Value);
    }

    /// <summary>
    /// 소환 게이지 회복 속도 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustSummonGaugeRecoverySpeed(float amount)
    {
        SummonGaugeRecoverySpeed = AdjustValue(SummonGaugeRecoverySpeed, amount, float.MaxValue);
    }

    /// <summary>
    /// 마나 게이지 회복 속도 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustQueenActiveSkillGaugeRecoverySpeed(float amount)
    {
        QueenActiveSkillGaugeRecoverySpeed = AdjustValue(QueenActiveSkillGaugeRecoverySpeed, amount, float.MaxValue);
    }

    /// <summary>
    /// 경험치 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustCurExpGauge(float amount)
    {
        float adjustedAmount = amount * ExpGainMultiplier;
        float temp = CurExpGauge.Value + adjustedAmount;

        while (temp >= MaxExpGauge.Value)
        {
            LevelUp();
            temp -= MaxExpGauge.Value;
        }

        CurExpGauge.Value = temp;
    }

    /// <summary>
    /// 레벨업 처리 및 강화 트리거 호출
    /// </summary>
    private void LevelUp()
    {
        Level.Value++;
        QueenEnhanceManager.Instance.ActivateEnhance();
    }

    /// <summary>
    /// 진화 포인트 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustEvolutionPoint(float amount)
    {
        EvolutionPoint.Value = AdjustValue(EvolutionPoint.Value, amount, float.MaxValue);
    }

    /// <summary>
    /// 골드 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustGold(float amount)
    {
        float adjustedAmount = amount * GoldGainMultiplier;
        Gold.Value = AdjustValue(Gold.Value, adjustedAmount, float.MaxValue);
    }

    /// <summary>
    /// 경험치 획득량 증가 비율 설정 (% 단위)
    /// </summary>
    public void SetExpGainMultiplierPercent(float percent)
    {
        expGainMultiplierPercent = Mathf.Max(0f, percent);
    }

    /// <summary>
    /// 골드 획득량 증가 비율 설정 (% 단위)
    /// </summary>
    public void SetGoldGainMultiplierPercent(float percent)
    {
        goldGainMultiplierPercent = Mathf.Max(0f, percent);
    }


    // 값 조정
    private float AdjustValue(float cur, float amount, float max)
    {
        return Mathf.Clamp(cur + amount, 0f, max);
    }
}