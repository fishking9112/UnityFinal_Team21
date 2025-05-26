using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 모든 동적UI 생성 관리를 위한 Manager
/// </summary>
public class UIManager : MonoSingleton<UIManager>
{
    private Dictionary<string, BaseUI> activeUIs = new Dictionary<string, BaseUI>();

    /// <summary>
    /// UI를 로드하고 활성화합니다.
    /// </summary>
    /// <typeparam name="T">BaseUI를 상속받는 UI 타입</typeparam>
    /// <returns>로드된 UI</returns>
    public async Task<T> ShowUI<T>() where T : BaseUI
    {
        if (!AddressableManager.Instance.isInitDownload)
        {
            Utils.LogWarning($"아직 AddressableManager의 Init이 다운로드 되지 않았습니다");
            return null;
        }

        string uiName = typeof(T).Name;

        // 이미 활성화된 UI가 있다면 반환
        if (activeUIs.ContainsKey(uiName))
        {
            activeUIs[uiName].OnShow();
            return activeUIs[uiName] as T;
        }

        // 프리팹 로드
        // GameObject uiPrefab = Resources.Load<GameObject>($"{UIPrefabPath}{uiName}");
        GameObject uiPrefab = await AddressableManager.Instance.LoadAssetAsync<GameObject>($"{uiName}.prefab", ResourcePath.UI);

        if (uiPrefab == null)
        {
            Utils.LogWarning($"UIManager: {uiName} 프리팹을 찾을 수 없습니다.");
            return null;
        }

        // UI 생성 및 초기화
        GameObject uiInstance = Instantiate(uiPrefab, transform);
        T uiComponent = uiInstance.GetComponent<T>();
        if (uiComponent == null)
        {
            Utils.LogWarning($"UIManager: {uiName}에 {typeof(T).Name} 컴포넌트가 없습니다.");
            Destroy(uiInstance);
            return null;
        }

        activeUIs.Add(uiName, uiComponent);
        uiComponent.Initialize();
        uiComponent.OnShow();

        return uiComponent;
    }

    /// <summary>
    /// 특정 UI 비활성화
    /// </summary>
    /// <typeparam name="T">BaseUI를 상속받는 UI 타입</typeparam>
    public void HideUI<T>() where T : BaseUI
    {
        string uiName = typeof(T).Name;

        if (activeUIs.TryGetValue(uiName, out BaseUI ui))
        {
            ui.OnHide();
        }
        else
        {
            Utils.LogWarning($"UIManager: {uiName} UI가 활성화 상태가 아닙니다.");
        }
    }

    /// <summary>
    /// 팝업 호출
    /// </summary>
    /// <param name="title">팝업 제목</param>
    /// <param name="message">팝업 메시지</param>
    /// <param name="onConfirmAction">확인 버튼 동작</param>
    /// <param name="onCancelAction">취소 버튼 동작</param>
    public void ShowPopup(string title, string message, Action onConfirmAction, Action onCancelAction = null)
    {
        ShowPopupAsync(title, message, onConfirmAction, onCancelAction).Forget();
    }

    /// <summary>
    /// 팝업 호출
    /// </summary>
    /// <param name="title">팝업 제목</param>
    /// <param name="message">팝업 메시지</param>
    /// <param name="onConfirmAction">확인 버튼 동작</param>
    /// <param name="onCancelAction">취소 버튼 동작</param>
    public async UniTask ShowPopupAsync(string title, string message, Action onConfirmAction, Action onCancelAction = null)
    {
        var popup = await ShowUI<PopupUI>();
        popup.Setup(title, message, onConfirmAction, onCancelAction);
    }

    /// <summary>
    /// 툴팁 호출
    /// </summary>
    /// <param name="id">툴팁 id</param>
    /// <param name="forceRun">한번 보는 거지만 두번째 작동 할 때</param>
    /// <param name="onFinishAction">Finish 버튼 눌렸을 때</param>
    public void ShowTooltip(int id, bool forceRun = true, Action onFinishAction = null)
    {
        ShowTooltipAsync(id, forceRun, onFinishAction).Forget();
    }

    /// <summary>
    /// 툴팁 호출
    /// </summary>
    /// <param name="id">툴팁 id</param>
    /// <param name="forceRun">한번 보는 거지만 두번째 작동 할 때</param>
    /// <param name="onFinishAction">Finish 버튼 눌렸을 때</param>
    public async UniTask ShowTooltipAsync(int id, bool forceRun = true, Action onFinishAction = null)
    {
        // 한번 실행했다면 작동 X
        if (PlayerPrefs.GetInt(id.ToString()) == 1 && !forceRun)
            return;

        PlayerPrefs.SetInt(id.ToString(), 1); // 실행 기록 저장
        PlayerPrefs.Save(); // 저장 즉시 적용
        var popup = await ShowUI<ToolTipUI>();
        popup.Setup(id, onFinishAction);
    }

    /// <summary>
    /// 특정 UI 제거
    /// </summary>
    /// <typeparam name="T">BaseUI를 상속받는 UI 타입</typeparam>
    public void RemoveUI<T>() where T : BaseUI
    {
        string uiName = typeof(T).Name;

        if (activeUIs.TryGetValue(uiName, out BaseUI ui))
        {
            Destroy(ui.gameObject);
            activeUIs.Remove(uiName);
        }
        else
        {
            Utils.LogWarning($"UIManager: {uiName} UI가 활성화 상태가 아닙니다.");
        }
    }

    /// <summary>
    /// 모든 UI 제거
    /// </summary>
    public void ClearAllUI()
    {
        foreach (var ui in activeUIs.Values)
        {
            Destroy(ui.gameObject);
        }
        activeUIs.Clear();
    }
}