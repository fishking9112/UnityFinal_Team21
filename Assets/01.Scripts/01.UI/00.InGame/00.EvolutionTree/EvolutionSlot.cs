using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EvolutionSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public EvolutionTree evolutionTree;

    [SerializeField] private GameObject SelectedUI;
    [SerializeField] private Image slotIcon;
    [SerializeField] private Button slotButton;

    public int slotIndex;
    public MonsterInfo slotMonsterInfoData;

    private void OnEnable()
    {
        SelectedUI.SetActive(false);
    }

    // 슬롯에 정보를 넣어주는 함수
    public void SetSlot(EvolutionNode node)
    {
        if (!node.isUnlock)
        {
            return;
        }

        slotMonsterInfoData = node.monsterInfo;
        slotIcon.sprite = node.image.sprite;
        slotIcon.enabled = true;
        slotIcon.preserveAspect = true;
    }

    // 초기 슬롯 세팅에 사용되는 함수
    public void SetSlot(MonsterInfo info)
    {
        slotMonsterInfoData = info;
        slotIcon.sprite = DataManager.Instance.iconAtlas.GetSprite(info.icon);
        slotIcon.enabled = true;
        slotIcon.preserveAspect = true;
    }

    // 슬롯 초기화
    public void ClearSlot()
    {
        slotMonsterInfoData = null;
        slotIcon.sprite = null;
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
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (slotMonsterInfoData != null)
            {
                if (slotMonsterInfoData != null)
                {
                    ClearSlot();
                    evolutionTree.EvolutionTreeUI.RemoveQueenSlot(slotIndex);
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotMonsterInfoData != null)
        {
            EvolutionDragIcon dragIcon = evolutionTree.EvolutionTreeUI.EvolutionDragIcon;
            dragIcon.node = null;
            dragIcon.SlotNode = this.slotMonsterInfoData;
            dragIcon.OnBeginDrag();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        EvolutionDragIcon dragIcon = evolutionTree.EvolutionTreeUI.EvolutionDragIcon;
        dragIcon.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        EvolutionDragIcon dragIcon = evolutionTree.EvolutionTreeUI.EvolutionDragIcon;
        dragIcon.SlotNode = null;
        dragIcon.OnEndDrag();
    }

    public void OnDrop(PointerEventData eventData)
    {
        EvolutionDragIcon dragIcon = evolutionTree.EvolutionTreeUI.EvolutionDragIcon;

        if (dragIcon.node != null && dragIcon.node.isUnlock)
        {
            MonsterInfo draggedMonster = dragIcon.node.monsterInfo;

            // 드래그한 몬스터가 이미 슬롯에 있는지 확인
            EvolutionSlot sourceSlot = null;
            foreach (var slot in evolutionTree.EvolutionTreeUI.SlotList)
            {
                if (slot.slotMonsterInfoData == draggedMonster)
                {
                    sourceSlot = slot;
                    break;
                }
            }

            // 빈칸이면 등록, 이미 다른 슬롯에 있으면 이동
            if (slotMonsterInfoData == null)
            {
                slotMonsterInfoData = draggedMonster;
                slotIcon.sprite = DataManager.Instance.iconAtlas.GetSprite(draggedMonster.icon);
                slotIcon.enabled = true;

                if (sourceSlot != null)
                {
                    sourceSlot.ClearSlot();
                    evolutionTree.EvolutionTreeUI.RemoveQueenSlot(sourceSlot.slotIndex);
                }

                evolutionTree.EvolutionTreeUI.AddQueenSlot(slotMonsterInfoData, slotIndex);
            }
            // 빈칸이 아니면 덮어쓰기
            else
            {
                slotMonsterInfoData = draggedMonster;
                slotIcon.sprite = DataManager.Instance.iconAtlas.GetSprite(draggedMonster.icon);
                slotIcon.enabled = true;

                if (sourceSlot != null && sourceSlot != this)
                {
                    sourceSlot.ClearSlot();
                    evolutionTree.EvolutionTreeUI.RemoveQueenSlot(sourceSlot.slotIndex);
                }

                evolutionTree.EvolutionTreeUI.AddQueenSlot(slotMonsterInfoData, slotIndex);
            }

            dragIcon.node = null;
            dragIcon.OnEndDrag();
            return;
        }

        if (dragIcon.SlotNode != null)
        {
            MonsterInfo draggedMonster = dragIcon.SlotNode;

            // 드래그한 슬롯 찾기
            EvolutionSlot sourceSlot = null;
            foreach (var slot in evolutionTree.EvolutionTreeUI.SlotList)
            {
                if (slot.slotMonsterInfoData == draggedMonster)
                {
                    sourceSlot = slot;
                    break;
                }
            }

            if (sourceSlot == null) return;

            if (slotMonsterInfoData == null)
            {
                // 빈 슬롯이면 그냥 이동
                slotMonsterInfoData = draggedMonster;
                slotIcon.sprite = DataManager.Instance.iconAtlas.GetSprite(slotMonsterInfoData.icon);
                slotIcon.enabled = true;

                sourceSlot.ClearSlot();
                evolutionTree.EvolutionTreeUI.AddQueenSlot(slotMonsterInfoData, slotIndex);
                evolutionTree.EvolutionTreeUI.RemoveQueenSlot(sourceSlot.slotIndex);
            }
            else
            {
                // 이미 차있으면 서로 교환
                MonsterInfo temp = slotMonsterInfoData;

                slotMonsterInfoData = sourceSlot.slotMonsterInfoData;
                slotIcon.sprite = DataManager.Instance.iconAtlas.GetSprite(slotMonsterInfoData.icon);
                slotIcon.enabled = true;

                sourceSlot.slotMonsterInfoData = temp;
                if (temp != null)
                {
                    sourceSlot.slotIcon.sprite = DataManager.Instance.iconAtlas.GetSprite(temp.icon);
                    sourceSlot.slotIcon.enabled = true;
                }
                else
                {
                    sourceSlot.ClearSlot();
                }

                // 퀸 슬롯에도 적용
                evolutionTree.EvolutionTreeUI.AddQueenSlot(slotMonsterInfoData, slotIndex);
                evolutionTree.EvolutionTreeUI.AddQueenSlot(sourceSlot.slotMonsterInfoData, sourceSlot.slotIndex);
            }
        }
    }
}
