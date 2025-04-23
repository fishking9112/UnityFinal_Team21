using UnityEngine;
using UnityEngine.UI;

public class Castle : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    public CastleCondition condition;

    private ReactiveProperty<float> cur;
    private ReactiveProperty<float> max;

    private void Awake()
    {
        cur = condition.CurCastleHealth;
        max = condition.MaxCastleHealth;

        cur.AddAction(UpdateFill);
        max.AddAction(UpdateFill);

        UpdateFill(0);
    }

    private void Update()
    {
        RecoveryHealth();
    }

    // 체력 바 UI 갱신
    private void UpdateFill(float useless)
    {
        if (max == null || max.Value <= 0f)
        {
            return;
        }

        fillImage.fillAmount = Mathf.Clamp01(cur.Value / max.Value);
    }

    /// <summary>
    /// Castle에 데미지를 입힘
    /// </summary>
    /// <param name="amount"> 입힐 데미지 양 </param>
    public virtual void TakeDamaged(float amount)
    {
        condition.AdjustCurHealth(-amount);

        if (condition.CurCastleHealth.Value <= 0f)
        {
            Die();
        }
    }

    // 성이 죽었을 때 처리
    private void Die()
    {
        GameManager.Instance.GameOver();
    }

    // 자동 체력 회복
    private void RecoveryHealth()
    {
        if (condition.CurCastleHealth.Value <= 0f)
        {
            return;
        }

        condition.AdjustCurHealth(condition.CastleHealthRecoverySpeed * Time.deltaTime);
    }

    // 메모리 누수 방지
    private void OnDestroy()
    {
        if (cur != null)
        {
            cur.RemoveAction(UpdateFill);
        }

        if (max != null)
        {
            max.RemoveAction(UpdateFill);
        }
    }
}
