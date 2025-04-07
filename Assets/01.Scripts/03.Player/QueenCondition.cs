using UnityEngine;

public class QueenCondition : MonoBehaviour
{
    [SerializeField] private float curMagicGauge;
    public float CurMagicGauge
    {
        get => curMagicGauge;
    }

    [SerializeField] private float maxMagicGauge;
    public float MaxMagicGauge
    {
        get => maxMagicGauge;
    }

    [SerializeField] private float curSummonGauge;
    public float CurSummonGauge
    {
        get => curSummonGauge;
    }

    [SerializeField] private float maxSummonGauge;
    public float MaxSummonGauge
    {
        get => maxSummonGauge;
    }

    /// <summary>
    /// condition 수치를 조정하는 함수들
    /// </summary>
    /// <param name="amount"> 얼만큼 조정할 것인지(양수,음수 둘 다 가능) </param>

    public void AdjustCurMagicGauge(float amount)
    {
        curMagicGauge = AdjustValue(curMagicGauge, amount, maxMagicGauge);
    }

    public void AdjustMaxMagicGauge(float amount)
    {
        maxMagicGauge = AdjustValue(maxMagicGauge, amount, float.MaxValue);
        curSummonGauge = AdjustValue(curMagicGauge, 0, maxMagicGauge);
    }

    public void AdjustCurSummonGauge(float amount)
    {
        curSummonGauge = AdjustValue(curSummonGauge, amount, curSummonGauge);
    }

    public void AdjustMaxSummonGauge(float amount)
    {
        maxSummonGauge = AdjustValue(maxSummonGauge, amount, float.MaxValue);
        curSummonGauge = AdjustValue(curSummonGauge, 0, maxSummonGauge);
    }

    private float AdjustValue(float cur, float amount, float max)
    {
        return Mathf.Clamp(cur + amount, 0, max);
    }
}
