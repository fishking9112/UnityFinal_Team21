using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SoundManager : MonoSingleton<SoundManager>
{
    [Header("사용할 모든 오디오 클립")]
    [SerializeField] private AudioClip[] audioClips; // 오디오 클립 배열

    [Header("오브젝트 풀 세팅")]
    [SerializeField] private int poolSize = 10; // 최초 풀 크기
    private Queue<AudioSource> audioSourcePool; // 오브젝트 풀

    private float bgmVolume = 1f;
    private float sfxVolume = 1f; 
    public float BGMVolume => bgmVolume;
    public float SFXVolume => sfxVolume;

    private Dictionary<string, AudioClip> soundDict; // SFX와 BGM 저장용 Dictionary
    private AudioSource bgmPlayer; // BGM 재생용 AudioSource
    private Coroutine bgmFadeCoroutine;

    protected override void Awake()
    {
        base.Awake();
        InitSoundManager();
    }

    /// <summary>
    /// 오디오 클립을 Dictionary에 저장하고 BGM 플레이어를 설정
    /// </summary>
    private void InitSoundManager()
    {
        // Addressables 사용으로 임시 주석 처리
        // Dictionary 초기화
        soundDict = new Dictionary<string, AudioClip>();
        // foreach (var clip in audioClips)
        // {
        //     soundDict[clip.name] = clip;
        // }

        // BGM 플레이어 초기화
        bgmPlayer = gameObject.AddComponent<AudioSource>();
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;

        // 오브젝트 풀 초기화
        InitPool();
    }

    /// <summary>
    /// Addressables를 통해 지정된 라벨의 AudioClip 에셋들을 비동기 로드
    /// </summary>
    /// <param name="label">로드할 AudioClip들의 Addressables 라벨</param>
    public async UniTask LoadAudioClipsAsync(string label)
    {
        // 기존 클립을 유지한 채, 새로 로드한 클립만 추가
        await Addressables.LoadAssetsAsync<AudioClip>(label, clip =>
        {
            if (!soundDict.ContainsKey(clip.name))
            {
                soundDict[clip.name] = clip;
            }

        }).ToUniTask();

        Utils.Log($"사운드 클립 로딩 완료 (label: {label})");
    }

    /// <summary>
    /// 오브젝트 초기 풀 설정
    /// </summary>
    private void InitPool()
    {
        audioSourcePool = new Queue<AudioSource>();
        for (int i = 0; i < poolSize; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.enabled = false;
            audioSourcePool.Enqueue(source);
        }
    }

    /// <summary>
    /// 지정된 이름의 효과음을 재생합니다.
    /// </summary>
    /// <param name="soundName">재생할 효과음의 이름</param>
    public void PlaySFX(string soundName)
    {
        if (soundDict.TryGetValue(soundName, out var clip))
        {
            AudioSource source = GetAvailableAudioSource();
            source.clip = clip;
            source.volume = sfxVolume;
            source.enabled = true;
            source.Play();

            // UniTask 사용하여 풀에 반환
            ReturnToPoolAsync(source, clip.length).Forget();
        }
        else
        {
            Utils.LogWarning($"SFX {soundName} not found");
        }
    }

    /// <summary>
    /// 사용 가능한 오디오 소스를 반환합니다.
    /// 풀에서 오디오 소스를 가져오거나 없을 경우 새로운 AudioSource를 생성합니다.
    /// </summary>
    /// <returns>사용할 AudioSource</returns>
    private AudioSource GetAvailableAudioSource()
    {
        if (audioSourcePool.Count > 0)
        {
            return audioSourcePool.Dequeue();
        }
        else
        {
            AudioSource newSource = gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            audioSourcePool.Enqueue(newSource);
            return newSource;
        }
    }

    /// <summary>
    /// return 효과음 to Pool
    /// </summary>
    private async UniTaskVoid ReturnToPoolAsync(AudioSource source, float delay)
    {
        await UniTask.Delay((int)(delay * 1000));
        source.Stop();
        source.enabled = false;

        // 중복 추가 방지
        if (!audioSourcePool.Contains(source))
        {
            audioSourcePool.Enqueue(source);
        }
    }

    /// <summary>
    /// 배경 음악(BGM)을 즉시 재생
    /// </summary>
    /// <param name="bgmName">재생할 BGM의 이름</param>
    public void PlayBGM(string bgmName)
    {
        if (soundDict.TryGetValue(bgmName, out var clip))
        {
            if (bgmPlayer.clip != clip)
            {
                bgmPlayer.clip = clip;
                bgmPlayer.volume = bgmVolume;
                bgmPlayer.Play();
            }
        }
        else
        {
            Utils.LogWarning($"BGM {bgmName} not found");
        }
    }

    /// <summary>
    /// 배경 음악(BGM)을 서서히 변경
    /// </summary>
    /// <param name="newBgmName">변경할 BGM의 이름</param>
    /// <param name="fadeDuration">페이드 아웃,인 시간</param>
    public void ChangeBGM(string newBgmName, float fadeDuration = 1f)
    {
        if (soundDict.TryGetValue(newBgmName, out var newClip))
        {
            if (bgmPlayer.clip == newClip) return;

            if (bgmFadeCoroutine != null)
                StopCoroutine(bgmFadeCoroutine);

            bgmFadeCoroutine = StartCoroutine(FadeOutAndChangeBGM(newClip, fadeDuration));
        }
        else
        {
            Utils.LogWarning($"BGM '{newBgmName}' not found");
        }
    }

    /// <summary>
    /// 배경 음악(BGM)을 서서히 페이드 아웃합니다.
    /// </summary>
    /// <param name="fadeDuration">페이드 아웃 지속 시간</param>
    public void FadeOutBGM(float fadeDuration = 1.5f)
    {
        if (bgmFadeCoroutine != null)
            StopCoroutine(bgmFadeCoroutine);

        bgmFadeCoroutine = StartCoroutine(FadeOutBGMCoroutine(fadeDuration));
    }

    /// <summary>
    /// 현재 재생 중인 BGM을 서서히 페이드 아웃
    /// </summary>
    /// <param name="duration">페이드 아웃 지속 시간</param>
    private IEnumerator FadeOutBGMCoroutine(float duration)
    {
        float startVolume = bgmPlayer.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            bgmPlayer.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / duration);
            yield return null;
        }

        bgmPlayer.Stop();
        bgmPlayer.clip = null;
    }

    /// <summary>
    /// BGM을 서서히 페이드 아웃한 후 새로운 BGM을 페이드 인하여 변경
    /// </summary>
    /// <param name="newClip">변경할 새로운 BGM 오디오 클립</param>
    /// <param name="duration">페이드 아웃 및 페이드 인에 걸리는 시간</param>
    private IEnumerator FadeOutAndChangeBGM(AudioClip newClip, float duration)
    {
        float startVolume = bgmPlayer.volume;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            bgmPlayer.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / duration);
            yield return null;
        }

        bgmPlayer.Stop();
        bgmPlayer.clip = newClip;
        bgmPlayer.Play();

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            bgmPlayer.volume = Mathf.Lerp(0f, bgmVolume, elapsedTime / duration);
            yield return null;
        }
    }

    /// <summary>
    /// BGM 볼륨을 변경
    /// </summary>
    /// <param name="volume">설정할 볼륨 값 0~1</param>
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmPlayer.volume = bgmVolume;
    }

    /// <summary>
    /// 효과음 볼륨을 변경
    /// </summary>
    /// <param name="volume">설정할 볼륨 값 0~1</param>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }
}
