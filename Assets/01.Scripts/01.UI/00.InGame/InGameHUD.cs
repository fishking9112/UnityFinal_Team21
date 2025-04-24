using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InGameHUD : MonoBehaviour
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

    [Header("버튼")]
    [SerializeField] private Button pauseButton;

    public void UpdateLevelText(float level)
    {
        levelText.text = $"LV. {level}";
    }

    public void UpdateGoldText(float gold)
    {
        goldText.text = $"Gold. {Utils.GetThousandCommaText((int)gold)}";
    }

    public void BindGauges(QueenCondition condition)
    {
        summonGaugeUI.Bind(condition.CurSummonGauge, condition.MaxSummonGauge);
        queenActiveSkillGaugeUI.Bind(condition.CurQueenActiveSkillGauge, condition.MaxQueenActiveSkillGauge);
        expGaugeUI.Bind(condition.CurExpGauge, condition.MaxExpGauge);
    }

    public void BindTimer(ReactiveProperty<float> timer)
    {
        timerUI.Bind(timer);
    }


    public void PauseButton(UnityAction callback)
    {
        pauseButton.onClick.AddListener(callback);
    }
}