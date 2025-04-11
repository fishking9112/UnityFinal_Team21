using UnityEngine;

public class QueenCondition : MonoBehaviour
{
    public ReactiveProperty<float> CurMagicGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> MaxMagicGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> CurSummonGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> MaxSummonGauge { get; private set; } = new ReactiveProperty<float>();

    private void Awake()
    {
        CurMagicGauge.Value = 100f;
        MaxMagicGauge.Value = 100f;
        CurSummonGauge.Value = 100f;
        MaxSummonGauge.Value = 100f;
    }

    public void AdjustCurMagicGauge(float amount)
    {
        CurMagicGauge.Value = AdjustValue(CurMagicGauge.Value, amount, MaxMagicGauge.Value);
    }

    public void AdjustMaxMagicGauge(float amount)
    {
        MaxMagicGauge.Value = AdjustValue(MaxMagicGauge.Value, amount, float.MaxValue);
        CurMagicGauge.Value = AdjustValue(CurMagicGauge.Value, 0, MaxMagicGauge.Value);
    }

    public void AdjustCurSummonGauge(float amount)
    {
        CurSummonGauge.Value = AdjustValue(CurSummonGauge.Value, amount, MaxSummonGauge.Value);
    }

    public void AdjustMaxSummonGauge(float amount)
    {
        MaxSummonGauge.Value = AdjustValue(MaxSummonGauge.Value, amount, float.MaxValue);
        CurSummonGauge.Value = AdjustValue(CurSummonGauge.Value, 0, MaxSummonGauge.Value);
    }

    private float AdjustValue(float cur, float amount, float max)
    {
        return Mathf.Clamp(cur + amount, 0f, max);
    }
}