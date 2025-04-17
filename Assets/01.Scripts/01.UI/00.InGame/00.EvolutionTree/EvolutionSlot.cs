using UnityEngine;
using UnityEngine.UI;

public class EvolutionSlot : MonoBehaviour
{
    public EvolutionTree evolutionTree;

    [SerializeField] private Image slotIcon;
    [SerializeField] private Button slotButton;

    private EvolutionNode selectedMonster;

    private void Start()
    {
        slotButton.onClick.AddListener(OnClickSlot);
    }

    private void OnClickSlot()
    {
        if(selectedMonster != null)
        {
            ClearSlot();
            return;
        }

        EvolutionNode node = evolutionTree.selectedNode;

        if(node != null && node.isUnlock)
        {
            evolutionTree.RemovePreSlotData(node);
            SetSlot(node);
        }
    }

    public void SetSlot(EvolutionNode node)
    {
        if (!node.isUnlock)
        {
            return;
        }

        selectedMonster = node;
        slotIcon.sprite = node.image.sprite;
        slotIcon.enabled = true;
        slotIcon.preserveAspect = true;
    }

    public void ClearSlot()
    {
        selectedMonster = null;
        slotIcon.enabled = false;
    }

    public bool HasMonster(EvolutionNode node)
    {
        return selectedMonster == node;
    }
}
