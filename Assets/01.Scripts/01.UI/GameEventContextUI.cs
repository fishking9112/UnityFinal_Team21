using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameEventContextUI : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    [SerializeField] private TMP_Text contentText; // 말풍선 텍스트
    [SerializeField] private RectTransform bubbleRect; // 말풍선 배경
    [SerializeField] private Vector2 maxSize = new Vector2(270f, 120f); // 최대 크기
    [SerializeField] private Vector2 minSize = new Vector2(270f, 40f); // 최소 크기
    [SerializeField] private Vector2 padding = new Vector2(2.5f, 92.5f); // 패딩 (여백)

    private RectTransform textRect;

    private void Awake()
    {
        textRect = contentText.GetComponent<RectTransform>();
    }

    /// <summary>
    /// 텍스트 내용을 변경하고 말풍선 크기를 자동 조절하는 함수
    /// </summary>
    public void SetText(string message)
    {
        contentText.SetText(message);
        ResizeBubble();
    }

    /// <summary>
    /// 텍스트 크기에 맞춰 말풍선 크기 자동 조절
    /// </summary>
    private void ResizeBubble()
    {
        Vector2 textSize = contentText.GetPreferredValues(maxSize.x, maxSize.y);
        textSize.x = Mathf.Clamp(textSize.x, minSize.x, maxSize.x);
        textSize.y = Mathf.Clamp(textSize.y, minSize.y, maxSize.y);

        Vector2 rectSize = (textSize + padding);
        rectSize = new Vector2(300f, rectSize.y);
        bubbleRect.sizeDelta = rectSize;

        textRect.sizeDelta = textSize;
    }
}
