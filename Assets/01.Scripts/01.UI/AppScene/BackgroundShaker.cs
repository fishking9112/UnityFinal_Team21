using UnityEngine;
using DG.Tweening;

public class BackgroundShaker : MonoBehaviour
{
    [SerializeField] private float moveAmount = 5f;      // 흔들림 범위
    [SerializeField] private float duration = 0.5f;      // 한 번 흔들리는 데 걸리는 시간

    private RectTransform rectTransform;
    private Vector3 originalPos;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalPos = rectTransform.anchoredPosition;

        StartShaking();
    }

    void StartShaking()
    {
        ShakeLoop();
    }

    void ShakeLoop()
    {
        Vector2 randomOffset = Random.insideUnitCircle * moveAmount;

        rectTransform.DOAnchorPos(originalPos + (Vector3)randomOffset, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => ShakeLoop()); // 반복적으로 계속 흔들기
    }

    private void OnDisable()
    {
        rectTransform.DOKill(); // 안전하게 Tween 정리
    }
}
