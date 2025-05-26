using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

/// <summary>
/// 다운로드 바와 다운로드 용량을 나타내는 UI (BaseUI 아님)
/// </summary>
public class DownloadUI : MonoBehaviour
{
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TextMeshProUGUI progressInfoText;

    /// <summary>
    /// 다운로드 바 용량 확인
    /// </summary>
    /// <param name="progressPercent">0~1 퍼센트</param>
    /// <param name="maxProgress">MB단위의 크기</param>
    public void SetProgress(float progressPercent, float maxProgress)
    {
        progressSlider.value = progressPercent;

        progressInfoText.text = $"{(progressPercent * maxProgress):F2}/{maxProgress:F2} MB";
    }
}
