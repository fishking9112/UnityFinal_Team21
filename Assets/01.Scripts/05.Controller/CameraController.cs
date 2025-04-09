using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("카메라 이동")]
    public float moveSpeed;
    public float cameraEdge;
    public float moveSmoothValue;

    [Header("카메라 줌")]
    public float zoomSpeed;
    public float minZoom;
    public float maxZoom;
    public float zoomSmoothValue;

    private float targetZoom;
    private float zoomVelocity = 0f;
    private Vector3 moveVelocity = Vector3.zero;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        targetZoom = mainCamera.orthographicSize;
    }

    private void Update()
    {
        MoveCamera();
        ZoomCamera();
    }

    // 마우스의 위치에 따라 카메라가 움직임
    private void MoveCamera()
    {
        Vector3 moveDir = Vector3.zero;
        Vector2 mousePos = Input.mousePosition;

        if (mousePos.x <= cameraEdge)
        {
            moveDir.x = -1;
        }
        else if (mousePos.x >= Screen.width - cameraEdge)
        {
            moveDir.x = 1;
        }

        if (mousePos.y <= cameraEdge)
        {
            moveDir.y = -1;
        }
        else if (mousePos.y >= Screen.height - cameraEdge)
        {
            moveDir.y = 1;
        }

        Vector3 targetPos = mainCamera.transform.position + moveDir.normalized * moveSpeed;

        mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, targetPos, ref moveVelocity, moveSmoothValue);
    }

    public void OnZoomCamera(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        Vector2 scrollValue = context.ReadValue<Vector2>();

        targetZoom -= scrollValue.y * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }

    // 마우스 휠로 카메라 줌
    private void ZoomCamera()
    {
        mainCamera.orthographicSize = Mathf.SmoothDamp(mainCamera.orthographicSize, targetZoom, ref zoomVelocity, zoomSmoothValue);
    }
}
