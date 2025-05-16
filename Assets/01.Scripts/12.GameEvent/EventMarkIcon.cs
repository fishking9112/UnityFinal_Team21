using UnityEngine;

public class EventMarkIcon : MonoBehaviour
{
    private float riseMarkingTime = 1f;      // 커지는 시간
    private float maxMarkingTime = 5f;       // 총 지속 시간
    private float curMarkingTime = 0f;

    private float maxSize = 1.5f;            // 최대 크기

    [SerializeField] private Transform pivot;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Update()
    {
        curMarkingTime += Time.deltaTime;

        if (curMarkingTime <= riseMarkingTime)
        {
            // 커지는 구간 (0 ~ riseMarkingTime)
            float t = curMarkingTime / riseMarkingTime;
            float scale = Mathf.Lerp(1f, maxSize, t);
            pivot.localScale = Vector3.one * scale;
        }
        else if (curMarkingTime <= maxMarkingTime)
        {
            // 작아지고 투명해지는 구간 (riseMarkingTime ~ maxMarkingTime)
            float t = (curMarkingTime - riseMarkingTime) / (maxMarkingTime - riseMarkingTime);
            float scale = Mathf.Lerp(maxSize, 0f, t);
            pivot.localScale = Vector3.one * scale;

            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(1f, 0f, t);
                spriteRenderer.color = color;
            }
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
