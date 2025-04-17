using UnityEngine;

public class QueenCondition : MonoBehaviour
{
    public ReactiveProperty<float> CurMagicGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> MaxMagicGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> CurSummonGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> MaxSummonGauge { get; private set; } = new ReactiveProperty<float>();

    [SerializeField] private float summonGaugeRecoverySpeed = 10f;
    public float SummonGaugeRecoverySpeed { get; private set; }


    [SerializeField] private float magicGaugeRecoverySpeed = 5f;
    public float MagicGaugeRecoverySpeed { get; private set; }


    private void Awake()
    {
        CurMagicGauge.Value = 100f;
        MaxMagicGauge.Value = 100f;
        CurSummonGauge.Value = 100f;
        MaxSummonGauge.Value = 100f;
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

    // 값 조정
    private float AdjustValue(float cur, float amount, float max)
    {
        return Mathf.Clamp(cur + amount, 0f, max);
    }
}