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
    public bool isSelectable = false;

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

        iconImage.sprite = DataManager.Instance.iconAtlas.GetSprite(info.Icon);
        enhanceNameText.text = info.name;

        enhanceNextLevelText.text = info.type == QueenEnhanceType.AddSkill ? string.Empty : (nextLevel >= info.maxLevel ? $"Lv. {nextLevel}(Max)" : $"Lv. {nextLevel}");

        enhanceTypeText.text = GetEnhanceTypeText(info.type);

        float previewValue = currentLevel == 0
            ? info.state_Base
            : info.state_Base + (info.state_LevelUp * currentLevel);

        string formattedValue = $"{previewValue * 100:F0}%";


        enhanceDecText.text = info.description.Replace("n", formattedValue);

        if (info.skill_ID != 0)
        {
            enhanceDecText.text += $"\n\n<color=#FFB600>* {DataManager.Instance.queenActiveSkillDic[info.skill_ID].name} : {DataManager.Instance.queenActiveSkillDic[info.skill_ID].description}</color>";
        }
    }

    /// <summary>
    /// 강화의 타입의 따라 표기 텍스트 분류
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private string GetEnhanceTypeText(QueenEnhanceType type)
    {
        return type switch
        {
            QueenEnhanceType.Point => "포인트",
            QueenEnhanceType.QueenPassive => "여왕 강화",
            QueenEnhanceType.MonsterPassive => "몬스터 강화",
            QueenEnhanceType.AddSkill => "스킬 습득",
            _ => "알 수 없음"
        };
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
        if (isSelected || !HasEnhancePoint() || !isSelectable) return;

        isSelected = true;

        bool PassPopup = StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().queenEnhanceUI.ApplyInhance(currentInfo);

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
                    if (PassPopup)
                    {
                        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().queenEnhanceUI.CloseUI();
                    }
                });
            });

        if (PassPopup)
        {
            GameManager.Instance.queen.condition.EnhancePoint--;
        }
    }
}
