using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class QueenEnhanceUI : MonoBehaviour
{
    [SerializeField] private QueenEnhanceStatusUI queenEnhanceStatusUI;
    [SerializeField] private SelectInhanceItem[] itemSlots;

    /// <summary>
    /// 자동으로 참조를 할당하고 초기 비활성화 처리
    /// </summary>
    // private void OnValidate()
    // {
    //     if (contentParent == null) return;

    //     int childCount = contentParent.childCount;
    //     itemSlots = new SelectInhanceItem[childCount];

    //     for (int i = 0; i < childCount; i++)
    //     {
    //         itemSlots[i] = contentParent.GetChild(i).GetComponent<SelectInhanceItem>();
    //     }

    //     objectParent = transform.GetChild(0).gameObject;
    //     objectParent.SetActive(false);
    // }

    /// <summary>
    /// 선택 UI를 표시하고 강화 항목 정보를 슬롯에 할당합니다.
    /// </summary>
    public void ShowSelectUI(List<QueenEnhanceInfo> list)
    {
        InGameUIManager.Instance.ShowWindow<QueenEnhanceController>();
        queenEnhanceStatusUI.RefreshStatus();
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].SetInfo(list[i]);
        }
    }

    /// <summary>
    /// 강화 선택창 UI 비활성화
    /// </summary>
    public void CloseUI()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].ResetButton();
        }
        InGameUIManager.Instance.HideWindow();
    }
}
