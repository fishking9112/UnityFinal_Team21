using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 제목, 설명, 확인, 취소 버튼이 있는 팝업UI
/// </summary>
public class ToolTipUI : BaseUI
{
    [SerializeField] private Image tooltipImg;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI decsText;
    [SerializeField] private TextMeshProUGUI pageText;

    [SerializeField] private Button finishButton;
    private Action onFinishAction;

    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;
    private List<int> historyList = new(); // 이전 단계 추적
    private int curPage = 0;

    /// <summary>
    /// 팝업 초기화
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        if (finishButton != null)
            finishButton.onClick.AddListener(() => OnClickFinish());

        if (nextButton != null)
            nextButton.onClick.AddListener(() => OnClickNext());

        if (prevButton != null)
            prevButton.onClick.AddListener(() => OnClickPrev());
    }

    /// <summary>
    /// 팝업 설정
    /// </summary>
    public void Setup(int id, Action onFinishAction = null)
    {
        this.onFinishAction = onFinishAction;
        InitPageHistory(id);
        UpdatePage(0);
    }

    public void InitPageHistory(int id)
    {
        historyList.Clear();
        int nextId = id;
        while (nextId != -1)
        {
            historyList.Add(DataManager.Instance.toolTipDic[nextId].id);
            nextId = DataManager.Instance.toolTipDic[nextId].nextId;
        }

    }

    public void UpdatePage(int curPage)
    {
        this.curPage = curPage;
        ToolTipUpdate();
        ButtonUpdate();
    }

    public void ToolTipUpdate()
    {
        int curId = historyList[curPage];
        if (titleText != null)
            titleText.text = DataManager.Instance.toolTipDic[curId].name;

        if (decsText != null)
            decsText.text = DataManager.Instance.toolTipDic[curId].description;

        if (pageText != null)
            pageText.text = $"{curPage + 1} / {historyList.Count}";

        if (tooltipImg != null && DataManager.Instance.toolTipDic[curId].image != String.Empty)
            tooltipImg.sprite = DataManager.Instance.tooltipAtlas.GetSprite(DataManager.Instance.toolTipDic[curId].image);
    }

    public void ButtonUpdate()
    {
        if (curPage < historyList.Count - 1)
        {
            nextButton.gameObject.SetActive(true);
            finishButton.gameObject.SetActive(false);
        }
        else
        {
            nextButton.gameObject.SetActive(false);
            finishButton.gameObject.SetActive(true);
        }
        if (0 < curPage)
            prevButton.gameObject.SetActive(true);
        else
            prevButton.gameObject.SetActive(false);

    }

    /// <summary>
    /// 확인 버튼 동작
    /// </summary>
    private void OnClickFinish()
    {
        LogManager.Instance.LogEvent(GameLog.Contents.Funnel, (int)GameLog.FunnelType.Tutorial);

        onFinishAction?.Invoke();
        OnHide();
    }

    /// <summary>
    /// 다음 버튼 동작
    /// </summary>
    private void OnClickNext()
    {
        if (curPage < historyList.Count - 1)
        {
            UpdatePage(curPage + 1);
        }
    }

    /// <summary>
    /// 이전 버튼 동작
    /// </summary>
    private void OnClickPrev()
    {
        if (curPage > 0)
        {
            UpdatePage(curPage - 1);
        }
    }
}
