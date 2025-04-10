using UnityEngine;
using UnityEngine.UI;

public class GaugeUI : MonoBehaviour
{
    private Image fillImage;

    private ReactiveProperty<float> cur;
    private ReactiveProperty<float> max;

    private void Awake()
    {
        fillImage = GetComponent<Image>();
    }


    public void Bind(ReactiveProperty<float> curGauge, ReactiveProperty<float> maxGauge)
    {
        cur = curGauge;
        max = maxGauge;

        cur.AddAction(UpdateFill);
        max.AddAction(UpdateFill);

        UpdateFill(cur.Value);
    }

    private void UpdateFill(float useless)
    {
        if (max == null || max.Value <= 0f)
        {
            return;
        }

        fillImage.fillAmount = Mathf.Clamp01(cur.Value / max.Value);
    }

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
