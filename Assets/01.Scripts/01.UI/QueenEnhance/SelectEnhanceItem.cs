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

    //private QueenEnhanceInfo currentInfo;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button selectButton;

    private Vector3 originalScale;
    private bool isSelected = false;

    private void Awake()
    {
        if (targetTransform == null) 
            targetTransform = transform;

        originalScale = targetTransform.localScale;
    }

   /* public void SetInfo(QueenInhanceInfo info)
    {
        currentInfo = info;
        titleText.text = info.name;
        descText.text = info.description;
        iconImage.sprite = null;
    }*/


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected) return;

        targetTransform.DOScale(originalScale * hoverScale, scaleDuration).SetEase(Ease.OutBack);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected) return;

        targetTransform.DOScale(originalScale, scaleDuration).SetEase(Ease.InOutSine);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isSelected) return;

        isSelected = true;
        targetTransform.DOScale(originalScale * 1.2f, 0.15f).SetEase(Ease.OutBounce)
            .OnComplete(() => {
                Utils.Log("능력 선택됨");
                    
                // 다른 선택지들은 비활성화하기


                // 연출을 위해 0.1초 정도 텀을 주고 종료
                DOVirtual.DelayedCall(0.1f, () =>
                {
                  //  QueenInhanceManager.Instance.ApplyInhance(currentInfo);
                  //  QueenInhanceManager.Instance.QueenInhanceUIController.CloseUI();

                });
            });
    }
}
