using UnityEngine;
using UnityEngine.UI;

public class EvolutionSlot : MonoBehaviour
{
    public EvolutionTree evolutionTree;

    [SerializeField] private Image slotIcon;
    [SerializeField] private Button slotButton;

    public int slotIndex;
    public EvolutionNode slotMonsterData;

    private void Start()
    {
        slotButton.onClick.AddListener(OnClickSlot);
    }

    private void OnClickSlot()
    {
        // 아무 몬스터를 선택하지 않고 슬롯을 클릭하면 해당 슬롯 초기화
        if(slotMonsterData != null)
        {
            ClearSlot();
            evolutionTree.RemoveQueenSlot(slotIndex);
            return;
        }

        EvolutionNode node = evolutionTree.selectedNode;

        if(node != null && node.isUnlock)
        {
            evolutionTree.RemovePreSlotData(node);
            SetSlot(node);
            evolutionTree.AddQueenSlot(node.monsterInfo, slotIndex);
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
}
