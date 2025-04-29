using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectInhanceItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float scaleDuration = 0.1f;

    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI enhanceNameText;
    [SerializeField] private TextMeshProUGUI enhanceNextLevelText;
    [SerializeField] private TextMeshProUGUI enhanceTypeText;
    [SerializeField] private TextMeshProUGUI enhanceDecText;

    private Transform targetTransform;
    private QueenEnhanceInfo currentInfo;
    private Vector3 originalScale;
    private bool isSelected = false;

    /// <summary>
    /// UI 요소와 기본 크기를 설정.
    /// </summary>
    private void Awake()
    {
        if (targetTransform == null)
            targetTransform = transform;

        originalScale = targetTransform.localScale;
    }

    /// <summary>
    /// 강화 항목 정보를 설정합니다.
    /// </summary>
    /// <param name="info">강화 항목 정보</param>
    public void SetInfo(QueenEnhanceInfo info)
    {
        currentInfo = info;

        int currentLevel = StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().queenEnhanceUI.GetEnhanceLevel(info.ID);
        int nextLevel = currentLevel + 1;

        iconImage.sprite = null;
        enhanceNameText.text = info.name;
        enhanceNextLevelText.text = nextLevel >= info.maxLevel ? $"Lv. {nextLevel}(Max)" : $"Lv. {nextLevel}";
        enhanceTypeText.text = info.type.ToString();

        float previewValue = currentLevel == 0
            ? info.state_Base
            : info.state_Base + (info.state_LevelUp * currentLevel);

        string formattedValue = QueenEnhanceStatusUI.PercentValueTypes.Contains(info.valueType)
            ? $"{previewValue * 100:F0}%"
            : $"{previewValue}";

        enhanceDecText.text = info.description.Replace("n", formattedValue);
    }

    /// <summary>
    /// 버튼 상태를 초기화합니다.
    /// </summary>
    public void ResetButton()
    {
        isSelected = false;
        targetTransform.localScale = originalScale;
    }

    /// <summary>
    /// 강화 포인트가 있는지 확인합니다.
    /// </summary>
    private bool HasEnhancePoint()
    {
        return GameManager.Instance.queen.condition.EnhancePoint > 0;
    }

    /// <summary>
    /// 마우스가 버튼에 들어왔을 때 호출되는 함수. 크기를 키웁니다.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected || !HasEnhancePoint()) return;

        targetTransform.DOScale(originalScale * hoverScale, scaleDuration).SetEase(Ease.OutBack).SetUpdate(true);
       
    }

    /// <summary>
    /// 마우스가 버튼에서 나갔을 때 호출되는 함수. 원래 크기로 복원합니다.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected || !HasEnhancePoint()) return;

        targetTransform.DOScale(originalScale, scaleDuration).SetEase(Ease.InOutSine).SetUpdate(true);
    }

    /// <summary>
    /// 버튼이 클릭되었을 때 호출되는 함수. 강화 항목을 선택합니다.
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelected || !HasEnhancePoint()) return;

        isSelected = true;

        targetTransform.DOScale(originalScale * 1.2f, 0.15f).SetEase(Ease.OutBounce).SetUpdate(true)
            .OnComplete(() =>
            {
                Utils.Log("능력 선택됨");

                // 선택된 후 원래 크기로 복원 (UI가 닫히기 전에)
                targetTransform.DOScale(originalScale, 0.1f).SetEase(Ease.InOutSine).SetUpdate(true);

                isSelected = false;

                // 연출을 위해 0.1초 정도 텀을 주고 종료
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().queenEnhanceUI.ApplyInhance(currentInfo);
                    StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().queenEnhanceUI.CloseUI();
                });
            });

        GameManager.Instance.queen.condition.EnhancePoint--;
    }
}
