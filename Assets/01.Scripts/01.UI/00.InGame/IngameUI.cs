using UnityEngine;

public class IngameUI : MonoBehaviour
{
    [Header("게이지")]
    [SerializeField] private GaugeUI magicGaugeUI;
    [SerializeField] private GaugeUI summonGaugeUI;

    [Header("타이머")]
    [SerializeField] private TimerUI timerUI;
    [SerializeField] private float limitTime = 1800f;

    private ReactiveProperty<float> CurTime = new ReactiveProperty<float>();
    private QueenCondition condition;

    private bool isTimeOver = false;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        condition = GameManager.Instance.queen.condition;

        summonGaugeUI.Bind(condition.CurSummonGauge, condition.MaxSummonGauge);
        magicGaugeUI.Bind(condition.CurMagicGauge, condition.MaxMagicGauge);

        CurTime.Value = limitTime;
        timerUI.Bind(CurTime);
    }

    private void Update()
    {
        OnTimer();
    }

    private void OnTimer()
    {
        if (isTimeOver)
        {
            return;
        }

        CurTime.Value -= Time.deltaTime;

        if (CurTime.Value <= 0f)
        {
            CurTime.Value = 0f;
            isTimeOver = true;

            Clear();
        }
    }

    private void Clear()
    {
        // 제한 시간 동안 버텼을 경우
    }
}
