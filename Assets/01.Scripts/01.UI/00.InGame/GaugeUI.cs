using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GaugeUI : MonoBehaviour
{
    private Image fillImage;
    private TextMeshProUGUI gaugeText;
    [SerializeField] private HPBarUI hpBarUI;

    private ReactiveProperty<float> cur;
    private ReactiveProperty<float> max;

    [Header("Gauge Image 점등")]
    private bool isImgFlash = false;
    [SerializeField] private Color flashColor = Color.white;// 깜빡거릴 색상 저장용
    [SerializeField] private int flashImgTime = 5; // 몇 번
    [SerializeField] private float duration = 1.0f; // 몇 초 동안
    [SerializeField] public float magnitude = 10f; // 흔들림 세기 (픽셀 단위)
    [SerializeField] public float current = 0f; // 흔들림 세기 (픽셀 단위)
    private Color originColor = Color.white; // 원본 색상 저장용
    private Vector2 originalPos = Vector2.zero;
    private Image flashImg;
    private Action flashAction;
    private CancellationTokenSource imgActionCts;

    private void Awake()
    {
        if (!TryGetComponent(out fillImage))
        {
            Utils.LogWarning("Image 컴포넌트를 찾을 수 없습니다.");
        }

        if (!TryGetComponent(out gaugeText))
        {
            Utils.LogWarning("TextMeshProUGUI 컴포넌트를 찾을 수 없습니다.");
        }

        if (!TryGetComponent(out flashImg))
        {
            Utils.LogWarning("Image 컴포넌트를 찾을 수 없습니다.");
        }
        else
        {
            originColor = flashImg.color;
            originColor.a = 1f; // 반드시 불투명하게 설정
            originalPos = flashImg.transform.position;
        }
    }

    // 반응형 프로퍼티로 현재 게이지와 최대 게이지를 바인딩
    public void Bind(ReactiveProperty<float> curGauge, ReactiveProperty<float> maxGauge, bool isImgFlash = false, Action flashAction = null)
    {
        Unbind();

        cur = curGauge;
        max = maxGauge;

        cur.AddAction(UpdateFill);
        max.AddAction(UpdateFill);
        this.isImgFlash = isImgFlash;
        this.flashAction = flashAction;

        current = cur.Value;
        UpdateFill(0);
    }

    // 반응형 프로퍼티의 값이 변경되면 실행할 함수. 값에 따라 이미지의 fillAmount가 바뀜
    private void UpdateFill(float useless)
    {
        if (max == null || max.Value <= 0f)
        {
            return;
        }

        if (isImgFlash && current > cur.Value)
        {
            ActionImg();
            flashAction?.Invoke();
        }

        current = cur.Value;
        fillImage.fillAmount = Mathf.Clamp01(cur.Value / max.Value);

        if (hpBarUI != null)
        {
            hpBarUI.UpdateHPBar(cur.Value, max.Value);
        }
    }


    // 반응형 프로퍼티로 현재 게이지와 최대 게이지를 바인딩 - Text
    public void BindText(ReactiveProperty<float> curGauge, ReactiveProperty<float> maxGauge)
    {
        Unbind();

        cur = curGauge;
        max = maxGauge;

        cur.AddAction(UpdateText);
        max.AddAction(UpdateText);

        UpdateText(0);
    }

    // 반응형 프로퍼티의 값이 변경되면 실행할 함수. 값에 따라 이미지의 Text가 바뀜
    private void UpdateText(float useless)
    {
        if (max == null || max.Value <= 0f)
        {
            return;
        }

        gaugeText.text = $"{cur.Value:F0} / {max.Value:F0}";
    }

    private void Unbind()
    {
        if (cur != null)
        {
            cur.RemoveAction(UpdateFill);
            cur.RemoveAction(UpdateText);
        }

        if (max != null)
        {
            max.RemoveAction(UpdateFill);
            max.RemoveAction(UpdateText);
        }
    }

    // 메모리 누수 방지용 자산해제
    private void OnDestroy()
    {
        Unbind();

        imgActionCts?.Cancel();
        imgActionCts?.Dispose();
        imgActionCts = null;
    }

    private void ActionImg()
    {
        imgActionCts?.Cancel();
        imgActionCts?.Dispose();
        // 새로운 CancellationTokenSource 만들기 (OnDisable용 토큰도 연동)
        imgActionCts = new CancellationTokenSource();

        ImgFlash();
        ImgShake();
    }

    // UniTask 실행 함수
    public void ImgFlash()
    {
        // Task 시작
        ImgFlashTask(imgActionCts.Token).Forget();
    }

    // UniTask 본문
    private async UniTaskVoid ImgFlashTask(CancellationToken token)
    {
        try
        {
            for (int i = 0; i < flashImgTime; i++)
            {
                flashImg.color = flashColor;
                await UniTask.Delay(TimeSpan.FromSeconds((duration / flashImgTime) * 0.8f), true, PlayerLoopTiming.Update, token);

                flashImg.color = originColor;
                await UniTask.Delay(TimeSpan.FromSeconds((duration / flashImgTime) * 0.2f), true, PlayerLoopTiming.Update, token);
            }
        }
        catch (OperationCanceledException)
        {
            Utils.Log("이미지 흔들리기 취소");
        }
        finally
        {
            flashImg.color = originColor; // 무조건 복원
        }
    }

    // UniTask 실행 함수
    public void ImgShake()
    {
        // Task 시작
        ImgShakeTask(imgActionCts.Token).Forget();
    }

    // UniTask 본문
    private async UniTaskVoid ImgShakeTask(CancellationToken token)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = UnityEngine.Random.Range(-1f, 1f) * magnitude;
            float offsetY = UnityEngine.Random.Range(-1f, 1f) * magnitude;

            flashImg.transform.position = originalPos + new Vector2(offsetX, offsetY);
            elapsed += Time.deltaTime;

            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken: token, true);
        }

        flashImg.transform.position = originalPos;
    }
}
