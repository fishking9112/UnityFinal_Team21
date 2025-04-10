using UnityEngine;
using TMPro;

public class TimerUI : MonoBehaviour
{
    private TextMeshProUGUI timerText;

    public ReactiveProperty<float> timeProperty;

    private void Awake()
    {
        timerText = GetComponent<TextMeshProUGUI>();
    }

    public void Bind(ReactiveProperty<float> time)
    {
        timeProperty = time;
        timeProperty.AddAction(UpdateTimerText);
        UpdateTimerText(timeProperty.Value);
    }

    private void UpdateTimerText(float time)
    {
        timerText.text = Utils.GetMMSSTime((int)time);
    }

    private void OnDestroy()
    {
        if (timeProperty != null)
        {
            timeProperty.RemoveAction(UpdateTimerText);
        }
    }
}
