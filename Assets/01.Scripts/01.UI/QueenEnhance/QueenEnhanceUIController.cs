using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QueenEnhanceUIController : MonoBehaviour
{

    [SerializeField] private GameObject rootUI;
    [SerializeField] private Transform contentParent;
    [SerializeField] private SelectInhanceItem[] itemSlots;

    /// <summary>
    /// 선택 UI를 표시하고 강화 항목 정보를 슬롯에 할당합니다.
    /// </summary>
   /* public void ShowSelectUI(List<QueenEnhanceInfo> list)
    {
        rootUI.SetActive(true);

        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < list.Count)
            {
                itemSlots[i].gameObject.SetActive(true);
                itemSlots[i].SetInfo(list[i]);
            }
            else
            {
                itemSlots[i].gameObject.SetActive(false);
            }
        }
    }*/

    public void CloseUI()
    {
        rootUI.SetActive(false);
    }
}
