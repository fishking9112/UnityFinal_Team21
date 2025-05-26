using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GaugeUI : MonoBehaviour
{
    private Image fillImage;
    private TextMeshProUGUI gaugeText;

    private ReactiveProperty<float> cur;
    private ReactiveProperty<float> max;

    private void Awake()
    {
        if (!TryGetComponent(out fillImage))
        {
            Utils.LogWarning("Image 컴포넌트를 찾을 수 없습니다.");
        }

        if (!TryGetComponent(out gaugeText))
        {
            Utils.LogWarning("TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.");
        }
    }

    // 반응형 프로퍼티로 현재 게이지와 최대 게이지를 바인딩
    public void Bind(ReactiveProperty<float> curGauge, ReactiveProperty<float> maxGauge)
    {
        Unbind();

        cur = curGauge;
        max = maxGauge;

        cur.AddAction(UpdateFill);
        max.AddAction(UpdateFill);

        UpdateFill(0);
    }

    // 반응형 프로퍼티의 값이 변경되면 실행할 함수. 값에 따라 이미지의 fillAmount가 바뀜
    private void UpdateFill(float useless)
    {
        if (max == null || max.Value <= 0f)
        {
            return;
        }

        fillImage.fillAmount = Mathf.Clamp01(cur.Value / max.Value);
    }


    // 반응형 프로퍼티로 현재 게이지와 최대 게이지를 바인딩 - Text
    public void BindText(ReactiveProperty<float> curGauge, ReactiveProperty<float> maxGauge)
    {
        Unbind();

        cur = curGauge;
        max = maxGauge;

        cur.AddAction(UpdateText);
        max.AddAction(UpdateText);

        UpdateText(0);
    }

    // 반응형 프로퍼티의 값이 변경되면 실행할 함수. 값에 따라 이미지의 Text가 바뀜
    private void UpdateText(float useless)
    {
        if (max == null || max.Value <= 0f)
        {
            return;
        }

        gaugeText.text = $"{cur.Value:F0} / {max.Value:F0}";
    }

    private void Unbind()
    {
        if (cur != null)
        {
            cur.RemoveAction(UpdateFill);
            cur.RemoveAction(UpdateText);
        }

        if (max != null)
        {
            max.RemoveAction(UpdateFill);
            max.RemoveAction(UpdateText);
        }
    }

    // 메모리 누수 방지용 자산해제
    private void OnDestroy()
    {
        Unbind();
    }
}
