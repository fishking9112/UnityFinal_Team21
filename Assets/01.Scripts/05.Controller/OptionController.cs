using UnityEngine;
using UnityEngine.UI;

public class OptionController : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button saveButton;
    [SerializeField] private Button cancelButton;

    private float tempBGMVolume;
    private float tempSFXVolume;

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
    }

    private void OnBGMVolumeChanged(float value)
    {
        tempBGMVolume = value;
        SoundManager.Instance.SetBGMVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        tempSFXVolume = value;
        SoundManager.Instance.SetSFXVolume(value);
    }

    private void SaveOptions()
    {
        // 여기에 저장 로직 추가
        gameObject.SetActive(false);
    }

    private void CancelOptions()
    {
        // 변경 사항 되돌리기
        SoundManager.Instance.SetBGMVolume(tempBGMVolume);
        SoundManager.Instance.SetSFXVolume(tempSFXVolume);
        bgmSlider.value = tempBGMVolume;
        sfxSlider.value = tempSFXVolume;

        gameObject.SetActive(false);
    }
}
