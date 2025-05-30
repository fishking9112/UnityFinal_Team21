using UnityEngine;

public class AttackMarkIcon : MonoBehaviour
{
    [SerializeField] private float shrinkDuration = 1f;      // 작아지는 시간
    [SerializeField] private float fadeOutDuration = 1f;     // 페이드 아웃 시간
    [SerializeField] private float minScale = 0.5f;          // 최종 크기

    private float curTime = 0f;

    [SerializeField] private Transform pivot;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void Update()
    {
        curTime += Time.deltaTime;

        if (curTime <= shrinkDuration)
        {
            // 작아지는 구간
            float t = curTime / shrinkDuration;
            float scale = Mathf.Lerp(1f, minScale, t);
            pivot.localScale = Vector3.one * scale;
        }
        else if (curTime <= shrinkDuration + fadeOutDuration)
        {
            // 고정된 크기 + 페이드 아웃
            pivot.localScale = Vector3.one * minScale;

            if (spriteRenderer != null)
            {
                float t = (curTime - shrinkDuration) / fadeOutDuration;
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
