using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Castle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Slider healthSlider;
    public CastleCondition condition;

    private ReactiveProperty<float> cur;
    private ReactiveProperty<float> max;

    [Header("빨간색 점등 관련 데이터")]
    private CancellationTokenSource _takeDamagedRendererCts;
    [SerializeField] private float takeDamagedRendererTimer = 0.5f;

    private void Awake()
    {
        GameManager.Instance.castle = this;
        QueenAbilityUpgradeManager.Instance.ApplyAllEffects();

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

        healthSlider.maxValue = max.Value;
        healthSlider.value = cur.Value;
    }

    /// <summary>
    /// Castle에 데미지를 입힘
    /// </summary>
    /// <param name="amount"> 입힐 데미지 양 </param>
    public virtual void TakeDamaged(float amount)
    {
        Vector2 randomOffset = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f));
        Vector3 worldPos = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
        if (condition.IsInvincible)
        {
            StaticUIManager.Instance.damageLayer.ShowDamage(0, worldPos + Vector3.up * 0.5f, fontSize: 1f);
            return;
        }
        StaticUIManager.Instance.damageLayer.ShowDamage(amount, worldPos + Vector3.up * 0.5f, fontSize: 1f);
        TakeDamagedRenderer();


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
        if (cur != null) cur.RemoveAction(UpdateFill);
        if (max != null) max.RemoveAction(UpdateFill);

        _takeDamagedRendererCts?.Cancel();
        _takeDamagedRendererCts?.Dispose();
        _takeDamagedRendererCts = null;
    }

    // UniTask 실행 함수
    public void TakeDamagedRenderer()
    {
        _takeDamagedRendererCts?.Cancel();
        _takeDamagedRendererCts?.Dispose();
        // 새로운 CancellationTokenSource 만들기 (OnDisable용 토큰도 연동)
        _takeDamagedRendererCts = new CancellationTokenSource();

        // Task 시작
        TakeDamagedRendererTask(_takeDamagedRendererCts.Token).Forget();
    }

    // UniTask 본문
    private async UniTaskVoid TakeDamagedRendererTask(CancellationToken token)
    {
        sprite.color = Color.red;

        await UniTask.Delay(TimeSpan.FromSeconds(takeDamagedRendererTimer), cancellationToken: token);

        sprite.color = Color.white;
    }
}
