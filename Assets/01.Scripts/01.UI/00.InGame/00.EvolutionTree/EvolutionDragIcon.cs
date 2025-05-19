using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EvolutionDragIcon : MonoBehaviour
{
    public EvolutionNode node;
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
        dragImage.sprite = node.image.sprite;
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
 