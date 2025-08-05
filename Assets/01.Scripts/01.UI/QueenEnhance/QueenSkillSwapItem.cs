using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QueenSkillSwapItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private int index;
    public int Index => index;

    [SerializeField] private QueenEnhanceUI queenEnhanceUI;
    [SerializeField] private GameObject SelectedUI;
    [SerializeField] private Image SkillIcon;

    [SerializeField] int skillID = 0;

    private void Awake()
    {
        this.queenEnhanceUI = StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().queenEnhanceUI;
    }

    private void OnEnable()
    {
        SelectedUI.SetActive(false);
    }

    private void OnDisable()
    {
        SelectedUI.SetActive(false);
    }

    public void SetSkillinfo(int skillID)
    {
        if( skillID == -1)
        {
            SkillIcon.enabled = false;
            return;
        }

        SkillIcon.enabled = true;

        this.skillID = skillID;

        SkillIcon.sprite = DataManager.Instance.iconAtlas.GetSprite(QueenActiveSkillManager.Instance.queenActiveSkillDic[skillID].info.Icon);
    }

    /// <summary>
    /// 마우스가 버튼에 들어왔을 때 호출되는 함수.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        SelectedUI.SetActive(true);
    }

    /// <summary>
    /// 마우스가 버튼에서 나갔을 때 호출되는 함수.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        SelectedUI.SetActive(false);
    }

    /// <summary>
    /// 버튼이 클릭되었을 때 호출되는 함수. 
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        queenEnhanceUI.SwapClickEvent(Index, skillID);
    }
}
