using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[RequireComponent(typeof(Image))]
public class ImageAlphaLooper : MonoBehaviour
{
    private Image targetImage;
    private Tween alphaTween;

    [Header("Alpha Animation Settings")]
    public float duration = 1.5f;
    public float minAlpha = 0f;
    public float maxAlpha = 1f;

    private void Awake()
    {
        targetImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartAlphaLoop();
    }

    private void OnDisable()
    {
        alphaTween?.Kill();
        ResetAlpha();
    }

    private void StartAlphaLoop()
    {
        Color color = targetImage.color;
        color.a = maxAlpha;
        targetImage.color = color;

        alphaTween = targetImage.DOFade(minAlpha, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetUpdate(true);
    }

    private void ResetAlpha()
    {
        Color color = targetImage.color;
        color.a = maxAlpha;
        targetImage.color = color;
    }
}
