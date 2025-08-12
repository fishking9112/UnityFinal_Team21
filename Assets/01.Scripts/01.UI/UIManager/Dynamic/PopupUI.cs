using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.Localization.Components;

/// <summary>
/// 제목, 설명, 확인, 취소 버튼이 있는 팝업UI
/// </summary>
public class PopupUI : BaseUI
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private LocalizeStringEvent titleLocalize;
    [SerializeField] private LocalizeStringEvent messageLocalize;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private Action onConfirm;
    private Action onCancel;

    /// <summary>
    /// 팝업 초기화
    /// </summary>
    public override void Initialize()
    {
        base.Initialize();

        if (confirmButton != null)
            confirmButton.onClick.AddListener(Confirm);

        if (cancelButton != null)
            cancelButton.onClick.AddListener(Cancel);
    }

    /// <summary>
    /// 팝업 설정 (일반 텍스트용)
    /// </summary>
    public void Setup(string title, string message, Action onConfirmAction, Action onCancelAction = null)
    {
        if (titleText != null)
            titleText.text = title;

        if (messageText != null)
            messageText.text = message;

        ConfigureButtons(onConfirmAction, onCancelAction);
    }

    /// <summary>
    /// 팝업 설정 (현지화 텍스트용)
    /// </summary>
    public void SetupLocalization(string titleID, string messageID, Action onConfirmAction, Action onCancelAction = null)
    {
        StringManager.Instance.SetString(titleID, titleLocalize);
        StringManager.Instance.SetString(messageID, messageLocalize);

        ConfigureButtons(onConfirmAction, onCancelAction);
    }

    /// <summary>
    /// 버튼의 상태와 액션을 설정
    /// </summary>
    private void ConfigureButtons(Action onConfirmAction, Action onCancelAction)
    {
        onConfirm = onConfirmAction;
        onCancel = onCancelAction;

        // 팝업이 재사용될 경우를 대비해 항상 버튼 상태를 초기화
        if (cancelButton != null)
        {
            cancelButton.gameObject.SetActive(true);
        }

        // 취소 액션이 제공되지 않으면 취소 버튼을 숨김
        if (onCancelAction == null)
        {
            if (cancelButton != null)
            {
                cancelButton.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 확인 버튼 동작
    /// </summary>
    private void Confirm()
    {
        onConfirm?.Invoke();
        OnHide();
    }

    /// <summary>
    /// 취소 버튼 동작
    /// </summary>
    private void Cancel()
    {
        onCancel?.Invoke();
        OnHide();
    }
}
