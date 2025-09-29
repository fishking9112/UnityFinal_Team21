using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class OptionController : MonoBehaviour
{
    [SerializeField] private GameObject OptionPanelUI;

    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TMP_Dropdown languageDropdown;
    [SerializeField] private TMP_Dropdown modeDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Button showCredits;

    private float tempBGMVolume;
    private float tempSFXVolume;

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

        // 모드 관련 초기화
        modeDropdown.ClearOptions();
        modeDropdown.AddOptions(new List<TMP_Dropdown.OptionData> {
            new TMP_Dropdown.OptionData(),
            new TMP_Dropdown.OptionData()
        });
        modeDropdown.value = Screen.fullScreen ? 0 : 1;
        modeDropdown.onValueChanged.AddListener(OnModeChanged);
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        RefreshModeDropdown().Forget();

        // 해상도 관련 초기화
        currentFullScreenResolution = Screen.currentResolution;
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

        BuildResolutionOptions();
        SyncResolutionDropdown();

        // 변경 감시
        lastResolution = new Resolution { width = Screen.width, height = Screen.height };
        lastFullScreen = Screen.fullScreen;
        WatchResolutionChangeAsync(this.GetCancellationTokenOnDestroy()).Forget();

        showCredits.onClick.AddListener(() => SceneLoadManager.Instance.LoadScene(LoadSceneEnum.CreditsScene).Forget());
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

    private void BuildResolutionOptions()
    {
        resolutions.Clear();

        foreach (Resolution res in Screen.resolutions)
        {
            if (res.width >= 1280)
            {
                if (!resolutions.Exists(r => r.width == res.width && r.height == res.height))
                {
                    resolutions.Add(res);
                }
            }
        }

        resolutions.Sort((a, b) => a.width.CompareTo(b.width));

        resolutionDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (var res in resolutions)
        {
            string aspectRatio = GetRatioString(res.width, res.height);
            options.Add(new TMP_Dropdown.OptionData($"{res.width} x {res.height} ({aspectRatio})"));
        }

        resolutionDropdown.AddOptions(options);
    }

    private void UpdateCurrentFullScreenResolution()
    {
        currentFullScreenResolution = Screen.currentResolution;
    }

    private void OnModeChanged(int index)
    {
        UpdateCurrentFullScreenResolution();
        int refreshRate = currentFullScreenResolution.refreshRate;

        if (index == 0) // 전체화면
        {
            Resolution res = currentFullScreenResolution;
            Screen.SetResolution(res.width, res.height, true, refreshRate);
        }
        else if (index == 1) // 창모드
        {
            resolutions.Sort((a, b) => a.width.CompareTo(b.width));
            Resolution lowerRes = resolutions.FindLast(r => r.width < currentFullScreenResolution.width);

            if (lowerRes.width > 0)
            {
                Screen.SetResolution(lowerRes.width, lowerRes.height, false, refreshRate);
            }
            else
            {
                Screen.SetResolution(currentFullScreenResolution.width, currentFullScreenResolution.height, false, refreshRate);
            }
        }

        SyncResolutionDropdown();
    }

    private void OnResolutionChanged(int index)
    {
        if (index < 0 || index >= resolutions.Count)
        {
            return;
        }

        Resolution selectedRes = resolutions[index];
        UpdateCurrentFullScreenResolution();
        int refreshRate = currentFullScreenResolution.refreshRate;

        // 모니터 기본보다 작으면 창모드
        bool fullscreen = selectedRes.width >= currentFullScreenResolution.width &&
                          selectedRes.height >= currentFullScreenResolution.height;

        // 모드 드롭다운 동기화
        modeDropdown.onValueChanged.RemoveListener(OnModeChanged);
        modeDropdown.value = fullscreen ? 0 : 1;
        modeDropdown.RefreshShownValue();
        modeDropdown.onValueChanged.AddListener(OnModeChanged);

        Screen.SetResolution(selectedRes.width, selectedRes.height, fullscreen, refreshRate);

        SyncResolutionDropdown();
    }

    private void SyncResolutionDropdown()
    {
        for (int i = 0; i < resolutions.Count; i++)
        {
            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
            {
                resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
                resolutionDropdown.value = i;
                resolutionDropdown.RefreshShownValue();
                resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
                return;
            }
        }
    }

    private async UniTaskVoid WatchResolutionChangeAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await UniTask.Delay(100, cancellationToken: token);

            if (Screen.width != lastResolution.width ||
                Screen.height != lastResolution.height ||
                Screen.fullScreen != lastFullScreen)
            {
                UpdateCurrentFullScreenResolution();
                SyncResolutionDropdown();

                lastResolution.width = Screen.width;
                lastResolution.height = Screen.height;
                lastFullScreen = Screen.fullScreen;
            }
        }
    }

    // 언어 변경 이벤트 발생 시 호출
    private void OnLocaleChanged(Locale locale)
    {
        RefreshModeDropdown().Forget();
    }

    // 모드 드롭다운 텍스트 갱신
    private async UniTaskVoid RefreshModeDropdown()
    {
        string fullscreen = await StringManager.Instance.GetString("9900067");
        string window = await StringManager.Instance.GetString("9900068");

        modeDropdown.options[0].text = fullscreen;
        modeDropdown.options[1].text = window;
        modeDropdown.RefreshShownValue();
    }


    // 가로세로 비율 문자열 반환
    private string GetRatioString(int width, int height)
    {
        float ratio = (float)width / height;

        // 어느정도 오차 허용을 통해 대표적인 비율로 매칭
        if (Mathf.Abs(ratio - 16f / 9f) < 0.01f)
        {
            return "16:9";
        }
        if (Mathf.Abs(ratio - 4f / 3f) < 0.01f)
        {
            return "4:3";
        }
        if (Mathf.Abs(ratio - 21f / 9f) < 0.01f)
        {
            return "21:9";
        }
        if (Mathf.Abs(ratio - 16f / 10f) < 0.01f)
        {
            return "16:10";
        }

        int gcd = GCD(width, height);
        return $"{width / gcd}:{height / gcd}";
    }

    // 비율 계산을 위한 최대공약수
    private int GCD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    private void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }
}
