using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class MiniMapClick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RawImage rawImage;
    public CameraController cameraController => GameManager.Instance.cameraController;
    private bool isDragging = false;

    private void Start()
    {
        Initialize().Forget();
    }


    public async UniTaskVoid Initialize()
    {
        // null이 아닐 때 까지 기다림
        await UniTask.WaitUntil(() => GameManager.Instance.cameraController.renderTexture != null)
        .Timeout(System.TimeSpan.FromSeconds(5)).SuppressCancellationThrow(); // 5초 넘어가면 에러 안 나고 그냥 끝냄

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