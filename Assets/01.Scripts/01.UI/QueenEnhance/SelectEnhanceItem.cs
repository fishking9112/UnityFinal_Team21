using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectInhanceItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Transform targetTransform;
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float scaleDuration = 0.1f;

    private QueenEnhanceInfo currentInfo;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI enhanceNameText;
    [SerializeField] private TextMeshProUGUI enhanceNextLevelText;
    [SerializeField] private TextMeshProUGUI enhanceTypeText;
    [SerializeField] private TextMeshProUGUI enhanceDecText;

    private Vector3 originalScale;
    private bool isSelected = false;

    private void Awake()
    {
        if (targetTransform == null) 
            targetTransform = transform;

        originalScale = targetTransform.localScale;
    }

    public void SetInfo(QueenEnhanceInfo info)
    {
        currentInfo = info;

        // 현재 강화 레벨 정보 조회
        int currentLevel = QueenEnhanceManager.Instance.GetEnhanceLevel(info.ID);
        int nextLevel = currentLevel + 1;

        iconImage.sprite = null;
        enhanceNameText.text = info.name;

        if (nextLevel >= info.maxLevel)
        {
            enhanceNextLevelText.text = $"Lv. {nextLevel}(Max)";
        }
        else
        {
            enhanceNextLevelText.text = $"Lv. {nextLevel}";
        }

        enhanceTypeText.text = info.type.ToString();

        int previewValue = info.state_Base + (info.state_LevelUp * currentLevel);
        enhanceDecText.text = info.description.Replace("n", previewValue.ToString());
    }
    public void ResetButton()
    {
        isSelected = false;
        targetTransform.localScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected) return;

        targetTransform.DOScale(originalScale * hoverScale, scaleDuration).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected) return;

        targetTransform.DOScale(originalScale, scaleDuration).SetEase(Ease.InOutSine).SetUpdate(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelected) return;

        isSelected = true;
        targetTransform.DOScale(originalScale * 1.2f, 0.15f).SetEase(Ease.OutBounce).SetUpdate(true)
            .OnComplete(() => {
                Utils.Log("능력 선택됨");

                // 다른 선택지들은 비활성화하기
                // 선택된 후 원래 크기로 복원 (UI가 닫히기 전에)
                targetTransform.DOScale(originalScale, 0.1f).SetEase(Ease.InOutSine).SetUpdate(true);

                isSelected = false;

                // 연출을 위해 0.1초 정도 텀을 주고 종료
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    QueenEnhanceManager.Instance.ApplyInhance(currentInfo);
                    QueenEnhanceManager.Instance.QueenEnhanceUIController.CloseUI();

                });
            });
    }
}
