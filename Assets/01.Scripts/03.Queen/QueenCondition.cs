using UnityEngine;

public class QueenCondition : MonoBehaviour
{
    [Header("초기 설정")]
    public float initSummonGaugeRecoverySpeed = 10f;
    public float initMagicGaugeRecoverySpeed = 5f;
    public float initCurMagicGauge = 100f;
    public float initMaxMagicGauge = 100f;
    public float initCurSummonGauge = 100f;
    public float initMaxSummonGauge = 100f;
    public float initCurExpGauge = 0f;
    public float initMaxExpGauge = 100f;
    public float initEvolutionPoint = 0f;
    public float initLevel = 1f;
    public float initGold = 0f;

    public float SummonGaugeRecoverySpeed { get; private set; }
    public float MagicGaugeRecoverySpeed { get; private set; }
    public ReactiveProperty<float> CurMagicGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> MaxMagicGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> CurSummonGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> MaxSummonGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> CurExpGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> MaxExpGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> EvolutionPoint { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> Level { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> Gold { get; private set; } = new ReactiveProperty<float>();

    private void Awake()
    {
        SummonGaugeRecoverySpeed = initSummonGaugeRecoverySpeed;
        MagicGaugeRecoverySpeed = initMagicGaugeRecoverySpeed;

        CurMagicGauge.Value = initCurMagicGauge;
        MaxMagicGauge.Value = initMaxMagicGauge;
        CurSummonGauge.Value = initCurSummonGauge;
        MaxSummonGauge.Value = initMaxSummonGauge;
        Level.Value = initLevel;
        CurExpGauge.Value = initCurExpGauge;
        MaxExpGauge.Value = initMaxExpGauge;
        EvolutionPoint.Value = initEvolutionPoint;
        Gold.Value = initGold;
    }
    private void Start()
    {
        ApplyAbilityUpgradeData();
    }

    /// <summary>
    /// 여왕 권능 강화 데이터 적용
    /// </summary>
    private void ApplyAbilityUpgradeData()
    {
       /* var upgrades = QueenAbilityUpgradeManager.Instance.GetAllStoredUpgrades();
        foreach (var upgrade in upgrades)
        {
            if (QueenAbilityUpgradeManager.Instance.TryGetApplyAction(upgrade.Key, out var action))
            {
                action.Invoke(upgrade.Value);
            }
        }*/
    }

    /// <summary>
    /// 현재 권능 게이지 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustCurMagicGauge(float amount)
    {
        CurMagicGauge.Value = AdjustValue(CurMagicGauge.Value, amount, MaxMagicGauge.Value);
    }

    /// <summary>
    /// 최대 권능 게이지 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustMaxMagicGauge(float amount)
    {
        MaxMagicGauge.Value = AdjustValue(MaxMagicGauge.Value, amount, float.MaxValue);
        CurMagicGauge.Value = AdjustValue(CurMagicGauge.Value, 0, MaxMagicGauge.Value);
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
        CurSummonGauge.Value = AdjustValue(CurSummonGauge.Value, 0, MaxSummonGauge.Value);
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
    public void AdjustMagicGaugeRecoverySpeed(float amount)
    {
        MagicGaugeRecoverySpeed = AdjustValue(MagicGaugeRecoverySpeed, amount, float.MaxValue);
    }

    /// <summary>
    /// 경험치 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustCurExpGauge(float amount)
    {
        float temp = CurExpGauge.Value + amount;

        while(temp >= MaxExpGauge.Value)
        {
            LevelUp();
            temp -= MaxExpGauge.Value;
        }

        CurExpGauge.Value = temp;
    }

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
        Gold.Value = AdjustValue(Gold.Value, amount, float.MaxValue);
    }

    // 값 조정
    private float AdjustValue(float cur, float amount, float max)
    {
        return Mathf.Clamp(cur + amount, 0f, max);
    }
}