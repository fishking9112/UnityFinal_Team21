using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapClick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RawImage rawImage;
    public CameraController cameraController => GameManager.Instance.cameraController;
    private bool isDragging = false;

    private void Start()
    {
        rawImage = GetComponent<RawImage>();
        rawImage.texture = GameManager.Instance.cameraController.renderTexture;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().isPaused)
        {
            return;
        }

        isDragging = true;
        GameManager.Instance.queen.controller.isMinimapDrag = true;
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
        GameManager.Instance.queen.controller.isMinimapDrag = false;
    }
}