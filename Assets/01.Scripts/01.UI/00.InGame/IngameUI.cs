using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class IngameUI : MonoBehaviour
{
    [Header("레벨")]
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("골드")]
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("게이지")]
    [SerializeField] private GaugeUI queenActiveSkillGaugeUI;
    [SerializeField] private GaugeUI summonGaugeUI;
    [SerializeField] private GaugeUI expGaugeUI;

    [Header("타이머")]
    [SerializeField] private TimerUI timerUI;
    [SerializeField] private float limitTime = 1800f;

    [Header("진화 트리")]
    [SerializeField] private GameObject evolutionTreeWindow;

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

        condition.Level.AddAction(UpdateLevelText);
        UpdateLevelText(condition.Level.Value);

        condition.Gold.AddAction(UpdateGoldText);
        UpdateGoldText(condition.Gold.Value);

        summonGaugeUI.Bind(condition.CurSummonGauge, condition.MaxSummonGauge);
        queenActiveSkillGaugeUI.Bind(condition.CurQueenActiveSkillGauge, condition.MaxQueenActiveSkillGauge);
        expGaugeUI.Bind(condition.CurExpGauge, condition.MaxExpGauge);

        CurTime.Value = limitTime;
        timerUI.Bind(CurTime);
    }

    private void Update()
    {
        OnTimer();
    }

    private void UpdateLevelText(float level)
    {
        levelText.text = $"LV. {level}";
    }

    private void UpdateGoldText(float gold)
    {
        goldText.text = $"Gold. {gold}";
    }

    // 제한 시간에서 점점 시간이 줄어들고 시간이 0이 됐을 때 클리어 판정
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

    public void OnEvolutionWindow(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            evolutionTreeWindow.SetActive(!evolutionTreeWindow.activeInHierarchy);
        }
    }

    // 메모리 누수 방지
    private void OnDestroy()
    {
        condition.Level.RemoveAction(UpdateLevelText);
    }
}
