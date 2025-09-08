using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EvolutionDragIcon : MonoBehaviour
{
    public EvolutionNode node;
    public MonsterInfo SlotNode;
    [SerializeField] private Image dragImage;

    private void Awake()
    {
        dragImage.enabled = false;
        dragImage.sprite = null;   
    }

    public void SetEvolutionNode(EvolutionNode node)
    {
        this.node = node;
    }

    public void OnBeginDrag()
    {
        if (node != null) // 진화 트리에서 드래그
        {
            dragImage.sprite = node.image.sprite;
        }
        else if (SlotNode != null) // 슬롯에서 드래그
        {
            dragImage.sprite = DataManager.Instance.iconAtlas.GetSprite(SlotNode.icon);
        }
        dragImage.enabled = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragImage != null)
            transform.position = eventData.position;
    }

    public void OnEndDrag()
    {
        dragImage.enabled = false;
        dragImage.sprite = null;
    }
}
 