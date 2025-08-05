using TMPro;
using UnityEngine;

public class BarUIText : MonoBehaviour
{
    [SerializeField] private RectTransform fillRect; // Fill 이미지의 RectTransform
    [SerializeField] private RectTransform textRect; // 텍스트의 RectTransform
    [SerializeField] private TextMeshProUGUI hpText;

    [Header("Text 위치 보정 값")]
    private float minXOffset = 10f; // 최소 좌측 위치 보정
    private float maxXOffset = -30f; // Fill 끝에서 텍스트 간격
    private float minDisplayWidth = 40f; // 최소 표시 너비 기준

    public void UpdateHPBar(float cur, float max)
    {
        float percent = Mathf.Clamp01(cur / max); // 실제 퍼센트 계산
        float fillWidth = fillRect.rect.width * percent; // 현재 fill 크기 기준 위치

        // fillWidth가 충분히 넓으면, 우측 끝에 위치 / 좁으면 내부 고정 위치
        float posX = (fillWidth > minDisplayWidth) ?
            fillWidth + maxXOffset : minXOffset;

        textRect.anchoredPosition = new Vector2(posX, textRect.anchoredPosition.y);
        hpText.text = $"{percent * 100f:0}%";
    }
}
