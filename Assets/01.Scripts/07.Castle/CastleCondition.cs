using UnityEngine;

public class CastleCondition : MonoBehaviour
{
    [Header("초기 설정")]
    public float initCastleHealthRecoverySpeed = 1f;
    public float initCurCastleHealth = 100f;
    public float initMaxCastleHealth = 100f;

    public float CastleHealthRecoverySpeed { get; private set; }
    public bool IsInvincible { get; private set; } = false;
    public ReactiveProperty<float> CurCastleHealth { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> MaxCastleHealth { get; private set; } = new ReactiveProperty<float>();

    private void Awake()
    {
        CurCastleHealth.Value = initCurCastleHealth;
        MaxCastleHealth.Value = initMaxCastleHealth;
        CastleHealthRecoverySpeed = initCastleHealthRecoverySpeed;
    }


    public void SetInvincible(bool value)
    {
        IsInvincible = value;
    }

    /// <summary>
    /// 현재 성 체력 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustCurHealth(float amount)
    {
        CurCastleHealth.Value = AdjustValue(CurCastleHealth.Value, amount, MaxCastleHealth.Value);
    }

    /// <summary>
    /// 최대 성 체력 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustMaxHealth(float amount)
    {
        MaxCastleHealth.Value = AdjustValue(MaxCastleHealth.Value, amount, float.MaxValue);
        CurCastleHealth.Value = AdjustValue(CurCastleHealth.Value, amount, MaxCastleHealth.Value);
    } 

    public void AdjustCastleHealthRecoverySpeed(float amount)
    {
        CastleHealthRecoverySpeed = AdjustValue(CastleHealthRecoverySpeed, amount, float.MaxValue);
    }

    private float AdjustValue(float cur, float amount, float max)
    {
        return Mathf.Clamp(cur + amount, 0f, max);
    }
}
