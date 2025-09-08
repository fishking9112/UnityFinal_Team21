using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OptionController : MonoBehaviour
{
    [SerializeField] private GameObject OptionPanelUI;

    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    private float tempBGMVolume;
    private float tempSFXVolume;
    private float currentScreenRatio;

    private List<Resolution> resolutions = new List<Resolution>();
    private Resolution currentFullScreenResolution;
    private Resolution lastResolution;
    private bool lastFullScreen;

    /// <summary>
    /// 초기 슬라이더 값 설정 및 이벤트 연결
    /// </summary>
    private void Start()
    {
        // 초기 슬라이더 값 설정
        tempBGMVolume = SoundManager.Instance.BGMVolume;
        tempSFXVolume = SoundManager.Instance.SFXVolume;

        bgmSlider.value = tempBGMVolume;
        sfxSlider.value = tempSFXVolume;

        // 슬라이더 이벤트 연결
        bgmSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);

        // 버튼 이벤트 연결
        saveButton.onClick.AddListener(SaveOptions);
        cancelButton.onClick.AddListener(CancelOptions);

        // 언어 드롭다운 연결
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        languageDropdown.value = StringManager.Instance.SelectLang;

        // 해상도 드롭다운 초기화
        currentFullScreenResolution = Screen.currentResolution;
        currentScreenRatio = (float)currentFullScreenResolution.width / currentFullScreenResolution.height;
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        InitResolutionDropdown();

        lastResolution = new Resolution { width = Screen.width, height = Screen.height };
        lastFullScreen = Screen.fullScreen;
        WatchResolutionChangeAsync(this.GetCancellationTokenOnDestroy()).Forget();

    }

    /// <summary>
    /// BGM 슬라이더 값이 변경되었을 때 호출됩니다.
    /// </summary>
    private void OnBGMVolumeChanged(float value)
    {
        SoundManager.Instance.SetBGMVolume(value);
    }

    /// <summary>
    /// SFX 슬라이더 값이 변경되었을 때 호출됩니다.
    /// </summary>
    private void OnSFXVolumeChanged(float value)
    {
        SoundManager.Instance.SetSFXVolume(value);
    }

    /// <summary>
    /// 현재 설정된 볼륨 값을 저장하고 옵션 창을 닫습니다.
    /// </summary>
    private void SaveOptions()
    {
        tempBGMVolume = SoundManager.Instance.BGMVolume;
        tempSFXVolume = SoundManager.Instance.SFXVolume;
        OptionPanelUI.SetActive(false);

        UGSManager.Instance.SaveLoad.SaveAsync().Forget();
    }

    /// <summary>
    /// 변경된 볼륨 값을 이전 값으로 되돌리고 옵션 창을 닫습니다.
    /// </summary>
    private void CancelOptions()
    {
        // 변경 사항 되돌리기
        SoundManager.Instance.SetBGMVolume(tempBGMVolume);
        SoundManager.Instance.SetSFXVolume(tempSFXVolume);
        bgmSlider.value = tempBGMVolume;
        sfxSlider.value = tempSFXVolume;

        OptionPanelUI.SetActive(false);
    }

    /// <summary>
    /// 변경된 언어 드롭다운의 인덱스 받기
    /// </summary>
    /// <param name="index"></param>

    public void OnLanguageChanged(int index)
    {
        StringManager.Instance.ChangeLocale(index);
    }

    private void InitResolutionDropdown()
    {
        resolutions.Clear();

        // 모니터에서 지원하는 최대 주사율 찾기
        int maxRefreshRate = 0;
        foreach (Resolution res in Screen.resolutions)
        {
            if (res.refreshRate > maxRefreshRate)
            {
                maxRefreshRate = res.refreshRate;
            }
        }

        foreach (Resolution res in Screen.resolutions)
        {
            float ratio = (float)res.width / res.height;

            // 현재 모니터 비율과 유사한 해상도만 추가 + 폭 1280이상 해상도만 지원 (너무 작아서 생기는 문제가 있을수도 있어서 미리 방지)
            if (Mathf.Abs(ratio - currentScreenRatio) < 0.01f &&
                res.width >= 1280 &&
                res.refreshRate == maxRefreshRate)
            {
                resolutions.Add(res);
            }
        }

        resolutionDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        int currentIndex = 0;

        for (int i = 0; i < resolutions.Count; i++)
        {
            Resolution res = resolutions[i];
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData
            {
                text = $"{res.width} x {res.height}"
            };
            options.Add(option);

            if (res.width == Screen.width && res.height == Screen.height)
            {
                currentIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void OnResolutionChanged(int index)
    {
        if (index < 0 || index >= resolutions.Count)
        {
            return;
        }

        Resolution selectedRes = resolutions[index];

        // 선택 해상도가 현재 디스플레이보다 크면 전체화면 유지
        bool fullscreen = selectedRes.width >= currentFullScreenResolution.width && selectedRes.height >= currentFullScreenResolution.height;

        Screen.SetResolution(selectedRes.width, selectedRes.height, fullscreen);
    }

    private async UniTaskVoid WatchResolutionChangeAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UniTask.Delay(1000, cancellationToken: token);

            if (Screen.width != lastResolution.width ||
                Screen.height != lastResolution.height ||
                Screen.fullScreen != lastFullScreen)
            {
                InitResolutionDropdown();

                lastResolution.width = Screen.width;
                lastResolution.height = Screen.height;
                lastFullScreen = Screen.fullScreen;
            }
        }
    }
}
