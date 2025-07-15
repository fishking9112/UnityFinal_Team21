using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class ToastMessage : MonoBehaviour
{
    [SerializeField] private TMP_Text contentText; // 말풍선 텍스트
    [SerializeField] private LocalizeStringEvent contentLocalize;
    [SerializeField] private RectTransform bubbleRect; // 말풍선 배경
    [SerializeField] private Vector2 maxSize = new Vector2(270f, 120f); // 최대 크기
    [SerializeField] private Vector2 minSize = new Vector2(270f, 30f); // 최소 크기
    [SerializeField] private Vector2 padding = new Vector2(2.5f, 92.5f); // 패딩 (여백)

    private RectTransform textRect;

    private Sequence seq;
    private Image[] img;
    private float duration;
    private RectTransform rect;

    private void Awake()
    {
        if (contentText == null)
        {
            Debug.LogWarning("ToastMessage: contentText가 Inspector에 할당되지 않았습니다. 할당해주세요.");
            return; // 필수 컴포넌트가 없으면 조기 종료
        }
        textRect = contentText.GetComponent<RectTransform>();
        img = GetComponentsInChildren<Image>();
        rect = GetComponent<RectTransform>();
        duration = 2f;
    }

    private void Start()
    {
        rect.position = Input.mousePosition;
        seq = DOTween.Sequence();

        seq.Join(transform.DOMoveY(rect.position.y + 10, duration));
        foreach (Image image in img)
        {
            seq.Join(image.DOFade(0f, duration));
        }
        seq.Join(contentText.DOFade(0f, duration));
        seq.OnComplete(() => Destroy(this.gameObject));
    }

    /// <summary>
    /// 텍스트 내용을 변경하고 말풍선 크기를 자동 조절하는 함수
    /// </summary>
    public void SetText(string messageID)
    {
        if (contentText == null || bubbleRect == null)
        {
            Debug.LogWarning("ToastMessage: contentText 또는 bubbleRect가 할당되지 않았습니다. 텍스트를 설정하거나 말풍선 크기를 조절할 수 없습니다.");
            return;
        }
        // contentText.SetText(message);
        StringManager.Instance.SetString(messageID, contentLocalize);

        ResizeBubble();
    }

    /// <summary>
    /// 텍스트 크기에 맞춰 말풍선 크기 자동 조절
    /// </summary>
    private void ResizeBubble()
    {
        if (contentText == null || bubbleRect == null || textRect == null)
        {
            return;
        }

        Vector2 textSize = contentText.GetPreferredValues(maxSize.x, maxSize.y);
        textSize.x = Mathf.Clamp(textSize.x, minSize.x, maxSize.x);
        textSize.y = Mathf.Clamp(textSize.y, minSize.y, maxSize.y);

        Vector2 rectSize = (textSize + padding);
        rectSize = new Vector2(300f, rectSize.y);
        bubbleRect.sizeDelta = rectSize;

        textRect.sizeDelta = textSize;
    }
}