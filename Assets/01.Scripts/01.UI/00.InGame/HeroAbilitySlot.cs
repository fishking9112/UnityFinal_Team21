using DG.Tweening;
using UnityEngine;

public class HeroAbilitySlot : MonoBehaviour
{
    private RectTransform target;

    [SerializeField] private float moveDuration = 1f;

    private bool isOpen = false;
    private Tween curTween;

    private void Awake()
    {
        target = GetComponent<RectTransform>();
    }

    public void OnClickToggleButton()
    {
        curTween?.Kill();

        float moveTarget = isOpen ? 550f : 0f;
        isOpen = !isOpen;

        curTween = target.DOAnchorPosX(moveTarget, moveDuration).SetEase(Ease.OutCubic);
    }
}
