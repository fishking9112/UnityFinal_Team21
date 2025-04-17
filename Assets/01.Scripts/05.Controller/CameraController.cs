using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("마왕성")]
    public GameObject castle;

    [Header("버츄얼 카메라")]
    public CinemachineVirtualCamera virtualCamera;

    [Header("미니맵")]
    public Camera miniMapCamera;
    public RectTransform miniMapRect;
    public GameObject miniMapIcon;

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
        UpdateMiniMapIconScale();
    }

    // 마우스의 위치에 따라 카메라가 움직임
    private void MoveCamera()
    {
        Vector3 moveDir = Vector3.zero;

        // 키보드의 입력을 우선으로 받음
        if (keyboardMoveDir != Vector2.zero)
        {
            moveDir = new Vector3(keyboardMoveDir.x, keyboardMoveDir.y, 0);
        }
        else
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

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

    public void OnMoveCamera(InputAction.CallbackContext context)
    {

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
            cameraTransform.position = castle.transform.position;
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

    // 카메라 시야범위를 미니맵에 그려줌
    private void UpdateMiniMapIconScale()
    {
        float cameraHeight = virtualCamera.m_Lens.OrthographicSize * 2;
        float cameraWidth = cameraHeight * ((float)Screen.width / Screen.height);

        miniMapIcon.transform.localScale = new Vector3(cameraWidth, cameraHeight, 1);
    }

    // 미니맵을 클릭하면 해당 위치로 카메라가 이동
    public void MiniMapClickCameraMove(Vector2 clickPosition)
    {
        Vector2 localPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(miniMapRect, clickPosition, null, out localPosition);

        // 미니맵 안의 위치를 0~1 범위로 정규화
        Vector2 normalizedLocalPosition = new Vector2(
            (localPosition.x / miniMapRect.rect.width) + miniMapRect.pivot.x,
            (localPosition.y / miniMapRect.rect.height) + miniMapRect.pivot.y
        );

        // 정규화된 position을 월드 좌표로 변환
        Vector3 worldPoint = miniMapCamera.ViewportToWorldPoint(normalizedLocalPosition);

        // 변환한 월드 좌표에 레이캐스트를 발사해서 충돌체가 있으면 그쪽으로 카메라 이동.(충돌체가 있어야 하기 때문에 맵에 콜라이더 필요함)
        RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

        if (hit.collider != null)
        {
            Vector3 target = hit.point;
            cameraTransform.position = new Vector3(target.x, target.y, cameraTransform.position.z);
        }
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
