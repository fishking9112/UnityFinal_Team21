using UnityEngine.EventSystems;
using UnityEngine;

public class MiniMapClick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public CameraController cameraController;
    private bool isDragging = false;

    private void Start()
    {
        if(cameraController == null)
        {
            cameraController = GameObject.Find("QueenCamera").GetComponent<CameraController>();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        cameraController.MiniMapClickCameraMove(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            cameraController.MiniMapClickCameraMove(eventData.position);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }
}