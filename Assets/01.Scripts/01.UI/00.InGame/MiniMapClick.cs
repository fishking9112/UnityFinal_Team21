using UnityEngine.EventSystems;
using UnityEngine;

public class MiniMapClick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public CameraController cameraController => GameManager.Instance.cameraController;
    private bool isDragging = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().isPaused)
        {
            return;
        }

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