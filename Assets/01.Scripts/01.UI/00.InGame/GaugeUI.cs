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

    // 반응형 프로퍼티로 현재 게이지와 최대 게이지를 바인딩
    public void Bind(ReactiveProperty<float> curGauge, ReactiveProperty<float> maxGauge)
    {
        cur = curGauge;
        max = maxGauge;

        cur.AddAction(UpdateFill);
        max.AddAction(UpdateFill);

        UpdateFill(cur.Value);
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

    // 메모리 누수 방지용 자산해제
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
