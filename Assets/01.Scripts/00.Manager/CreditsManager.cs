using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class CreditsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup teamLogo;
    public CanvasGroup titleInImg;
    public CanvasGroup titleOutImg;

    [Header("Fade Settings")]
    public float fadeTimer = 2.5f;

    [Header("Camera Movement")]
    public Transform cameraTransform;
    public GameObject endTargetY;
    public float moveSpeed = 2f;

    [Header("Hold to Load Scene")]
    public KeyCode keyToHold = KeyCode.Space;
    public float requiredHoldTime = 2f;
    public Image progressFill;

    // 현재 키를 누르고 있는 시간
    private float currentHoldTime = 0f;


    private async void Start()
    {
        // 시작 시 슬라이더를 비활성화하고 값을 초기화
        if (progressFill != null)
        {
            progressFill.fillAmount = 0;
            progressFill.gameObject.SetActive(false);
        }

        // 이 오브젝트가 파괴될 때 작업을 취소하기 위한 토큰
        var token = this.GetCancellationTokenOnDestroy();

        try
        {
            await FadeInAsync(fadeTimer, teamLogo, token);
            await UniTask.Delay(System.TimeSpan.FromSeconds(1.5f), cancellationToken: token);
            await FadeOutAsync(fadeTimer, teamLogo, token);
            await FadeInAsync(fadeTimer, titleInImg, token);
            await UniTask.Delay(System.TimeSpan.FromSeconds(1.5f), cancellationToken: token);

            await MoveCameraToYAsync(token);
            await FadeOutAsync(fadeTimer, titleOutImg, token);
            await UniTask.Delay(System.TimeSpan.FromSeconds(1.5f), cancellationToken: token);

            LoadTargetScene();
        }
        catch
        {

        }
    }

    private void Update()
    {
        // 지정된 키를 누르고 있는지 확인
        if (Input.GetKey(keyToHold))
        {
            currentHoldTime += Time.deltaTime;

            if (progressFill != null)
            {
                progressFill.gameObject.SetActive(true);
                progressFill.fillAmount = currentHoldTime / requiredHoldTime;
            }

            if (currentHoldTime >= requiredHoldTime)
            {
                LoadTargetScene();
            }
        }
        // 키에서 손을 떼면 모든 것을 초기화
        else if (Input.GetKeyUp(keyToHold))
        {
            currentHoldTime = 0f;
            if (progressFill != null)
            {
                progressFill.fillAmount = 0;
                progressFill.gameObject.SetActive(false);
            }
        }
    }

    private void LoadTargetScene()
    {
        // 중복 실행을 방지하기 위해 이 컴포넌트를 비활성화
        this.enabled = false;
        SceneLoadManager.Instance.LoadScene(LoadSceneEnum.MenuScene).Forget();
    }

    private async UniTask MoveCameraToYAsync(CancellationToken cancellationToken)
    {
        if (cameraTransform == null)
        {
            return;
        }

        float targetY = endTargetY.transform.position.y;

        while (Mathf.Abs(cameraTransform.position.y - targetY) > 0.01f)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
            Vector3 currentPosition = cameraTransform.position;
            Vector3 targetPosition = new Vector3(currentPosition.x, targetY, currentPosition.z);
            cameraTransform.position = Vector3.MoveTowards(currentPosition, targetPosition, moveSpeed * Time.deltaTime);
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
        cameraTransform.position = new Vector3(cameraTransform.position.x, targetY, cameraTransform.position.z);
    }

    private async UniTask FadeInAsync(float duration, CanvasGroup fadeCanvasGroup, CancellationToken cancellationToken)
    {
        float t = 0f;
        while (t < duration)
        {
            if (cancellationToken.IsCancellationRequested) return;
            t += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, t / duration);
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
        fadeCanvasGroup.alpha = 1f;
    }

    private async UniTask FadeOutAsync(float duration, CanvasGroup fadeCanvasGroup, CancellationToken cancellationToken)
    {
        float t = 0f;
        while (t < duration)
        {
            if (cancellationToken.IsCancellationRequested) return;
            t += Time.unscaledDeltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, t / duration);
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
        fadeCanvasGroup.alpha = 0f;
        fadeCanvasGroup.interactable = false;
        fadeCanvasGroup.blocksRaycasts = false;
    }
}
