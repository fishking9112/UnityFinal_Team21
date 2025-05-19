using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameTimerUI : MonoBehaviour
{
    [Header("타이머")]
    [SerializeField] private TextMeshProUGUI timerText;
    private ReactiveProperty<float> curTime => GameManager.Instance.curTime;

    public void Start()
    {
        curTime.AddAction(UpdateTimerText);
        UpdateTimerText(curTime.Value);
    }

    public void UpdateTimerText(float time)
    {
        timerText.text = Utils.GetMMSSTime((int)time);
    }

}
