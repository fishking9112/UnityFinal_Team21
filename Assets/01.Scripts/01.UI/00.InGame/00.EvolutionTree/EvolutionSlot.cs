using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EvolutionSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler, IPointerClickHandler
{
    public EvolutionTree evolutionTree;

    [SerializeField] private GameObject SelectedUI;
    [SerializeField] private Image slotIcon;
    [SerializeField] private Button slotButton;

    public int slotIndex;
    public EvolutionNode slotMonsterData;

    private void OnEnable()
    {
        SelectedUI.SetActive(false);
    }

    public void OnDrop(PointerEventData eventData)
    {
        var dragged = evolutionTree.EvolutionTreeUI.EvolutionDragIcon.GetComponent<EvolutionDragIcon>();

        if (dragged != null)
        {
            EvolutionNode node = dragged.node;

            if (node != null && node.isUnlock)
            {
                evolutionTree.RemovePreSlotData(node);
                SetSlot(node);
                evolutionTree.AddQueenSlot(node.monsterInfo, slotIndex);
            }
        }
    }

    // 슬롯에 정보를 넣어주는 함수
    public void SetSlot(EvolutionNode node)
    {
        if (!node.isUnlock)
        {
            return;
        }

        slotMonsterData = node;
        slotIcon.sprite = node.image.sprite;
        slotIcon.enabled = true;
        slotIcon.preserveAspect = true;
    }

    // 슬롯 초기화
    public void ClearSlot()
    {
        slotMonsterData = null;
        slotIcon.enabled = false;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            if(slotMonsterData != null)
            {
                if (slotMonsterData != null)
                {
                    ClearSlot();
                    evolutionTree.RemoveQueenSlot(slotIndex);
                }
            }
        }
    }
}
