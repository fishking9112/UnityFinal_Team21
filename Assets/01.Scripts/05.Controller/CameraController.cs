using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("용사")]
    public GameObject Hero;

    [Header("버츄얼 카메라")]
    public CinemachineVirtualCamera virtualCamera;

    [Header("카메라 이동")]
    public float cameraEdge;
    public float acceleration;
    public float maxMoveSpeed;

    [Header("카메라 이동 제한")]
    public Vector2 minRange;
    public Vector2 maxRange;

    [Header("카메라 줌")]
    public float zoomSpeed;
    public float minZoom;
    public float maxZoom;
    public float zoomSmoothValue;

    private float targetZoom;
    private float zoomVelocity;
    private Vector3 curSpeed;
    private Transform cameraTransform;

    private Vector2 keyboardMoveDir;

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

        if(keyboardMoveDir != Vector2.zero)
        {
            moveDir = new Vector3(keyboardMoveDir.x, keyboardMoveDir.y, 0);
        }
        else
        {
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

    // 키보드로 카메라 움직임
    public void OnKeyboradCameraMove(InputAction.CallbackContext context)
    {
        keyboardMoveDir = context.ReadValue<Vector2>();
    }

    // 마우스 휠 값을 받아옴
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

    // Spacebar 입력시 카메라를 히어로가 있는 쪽으로 옮김
    public void OnCameraMoveToHero(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            cameraTransform.position = Hero.transform.position;
            cameraTransform.position += new Vector3(0, 0, -10);
        }
    }

    // 카메라 범위 제한
    private void ClampCameraPosition()
    {
        if (virtualCamera == null)
        {
            return;
        }
        if (cameraTransform == null)
        {
            return;
        }

        float cameraHeight = virtualCamera.m_Lens.OrthographicSize;
        float cameraWidth = cameraHeight * ((float)Screen.width / Screen.height);

        Vector3 camPos = cameraTransform.position;

        float minX = minRange.x + cameraWidth;
        float maxX = maxRange.x - cameraWidth;
        float minY = minRange.y + cameraHeight;
        float maxY = maxRange.y - cameraHeight;

        camPos.x = Mathf.Clamp(camPos.x, minX, maxX);
        camPos.y = Mathf.Clamp(camPos.y, minY, maxY);

        camPos.z = cameraTransform.position.z;
        cameraTransform.position = camPos;
    }


    // 카메라 제한 범위 기즈모를 그려줌
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 center = new Vector3(
            (minRange.x + maxRange.x) / 2f,
            (minRange.y + maxRange.y) / 2f,
            0f
        );

        Vector3 size = new Vector3(
            Mathf.Abs(maxRange.x - minRange.x),
            Mathf.Abs(maxRange.y - minRange.y),
            0f
        );

        Gizmos.DrawWireCube(center, size);
    }
}
