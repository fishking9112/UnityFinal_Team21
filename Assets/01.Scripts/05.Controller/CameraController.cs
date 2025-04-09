using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("용사")]
    public GameObject Hero;

    [Header("버츄얼 카메라")]
    public CinemachineVirtualCamera virtualCamera;
    public Collider2D cameraLimitCollider;

    [Header("카메라 이동")]
    public float cameraEdge;
    public float acceleration;
    public float maxMoveSpeed;

    [Header("카메라 줌")]
    public float zoomSpeed;
    public float minZoom;
    public float maxZoom;
    public float zoomSmoothValue;

    private float targetZoom;
    private float zoomVelocity;
    private Vector3 curSpeed;
    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = virtualCamera.transform;
        targetZoom = virtualCamera.m_Lens.OrthographicSize;
    }

    private void LateUpdate()
    {
        MoveCamera();
        ZoomCamera();
        ClampCameraPosition();
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

        if (moveDir != Vector3.zero)
        {
            curSpeed += moveDir.normalized * acceleration * Time.deltaTime;
            curSpeed = Vector3.ClampMagnitude(curSpeed, maxMoveSpeed);
        }
        else
        {
            curSpeed = Vector3.zero;
        }

        cameraTransform.position += curSpeed * Time.deltaTime;
    }

    public void OnZoomCamera(InputAction.CallbackContext context)
    {
        Vector2 scrollValue = context.ReadValue<Vector2>();

        if (scrollValue.y == 0)
        {
            return;
        }

        targetZoom -= scrollValue.y * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
    }

    // 마우스 휠로 카메라 줌
    private void ZoomCamera()
    {
        virtualCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(virtualCamera.m_Lens.OrthographicSize,
                                                        targetZoom,
                                                        ref zoomVelocity,
                                                        zoomSmoothValue);
    }

    public void OnFixCamera(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            cameraTransform.position = Hero.transform.position;
            cameraTransform.position += new Vector3(0, 0, -10);
        }
    }

    // 카메라 범위 제한
    private void ClampCameraPosition()
    {
        Bounds bounds = cameraLimitCollider.bounds;

        Vector3 camPos = cameraTransform.position;
        float cameraHeight = virtualCamera.m_Lens.OrthographicSize;
        float cameraWidth = cameraHeight * (Screen.width / Screen.height);

        camPos.x = Mathf.Clamp(camPos.x, bounds.min.x + cameraWidth, bounds.max.x - cameraWidth);
        camPos.y = Mathf.Clamp(camPos.y, bounds.min.y + cameraHeight, bounds.max.y - cameraHeight);

        cameraTransform.position = camPos;
    }
}
