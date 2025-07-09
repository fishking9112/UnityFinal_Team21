using Cysharp.Threading.Tasks;
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

    private float tempBGMVolume;
    private float tempSFXVolume;

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

        StoveGameServiceManager.Instance.SaveLoad.SaveAsync().Forget();
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
}
