using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;


public class ResourcePath
{
    public const string Audio = "Audio";
    public const string Datas = "Datas";
    public const string Images = "Images";
    public const string Prefabs = "Prefabs";
    public const string Thumbnail = "Thumbnail";
    public const string Scene = "Scene";
    public const string UI = "UI";
}

/// <summary>
/// Addressable 에셋들 관리
/// </summary>
public class AddressableManager : MonoSingleton<AddressableManager>
{
    [SerializeField] private DownloadUI downloadUI; // 다운 받을게 있는 경우 활성화
    [HideInInspector] public bool isInitDownload = false; // InitDownload 로드 시 true
    private float allDownloadSize = 0; // 총 다운로드 받을 크기

    protected override void Awake()
    {
        base.Awake();
        DownloadAssets("InitDownload").Forget();
    }

    /// <summary>
    /// 에셋 로드
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">실제 파일 이름(ex, myPrefab.prefab)</param>
    /// <param name="type">addressable 설정 경로(ex, ResourcePath.UI)</param>
    /// <returns></returns>
    public T LoadAsset<T>(string key, string type) where T : UnityEngine.Object
    {
        var path = $"{type}/{key}";
        try
        {
            if (typeof(T).IsSubclassOf(typeof(MonoBehaviour)))
            {
                var obj = Addressables.LoadAssetAsync<GameObject>(path).WaitForCompletion();
                return obj.GetComponent<T>();
            }
            else
                return Addressables.LoadAssetAsync<T>(path).WaitForCompletion();
        }
        catch (Exception e)
        {
            Utils.Log($"로드 중 오류 발생: {e.Message}");
        }
        return default;
    }

    /// <summary>
    /// Addressable 에셋을 미리 다운로드 받아야 LoadAssetAsync 시 다운받는 딜레이 없이 사용 가능
    /// 다운받을 에셋 용량 확인
    /// </summary>
    /// <param name="label">다운받을 에셋의 라벨</param>
    /// <returns></returns>
    private async UniTask DownloadAssets(string label)
    {
        try
        {
            // label에 대해 다운받을 에셋 번들이 있는 지 용량 확인을 위함 
            var sizeHandle = Addressables.GetDownloadSizeAsync(label);

            // 다운로드 받을 Addressable 에셋들 확인
            await sizeHandle.ToUniTask();

            // 다운 받을 수 있을 경우
            if (sizeHandle.Status == AsyncOperationStatus.Succeeded)
            {
                downloadUI?.gameObject.SetActive(true);
                // 다운 받아야 할 에셋에 대한 크기가 0 이상인 경우
                if (sizeHandle.Result > 0)
                {
                    allDownloadSize = sizeHandle.Result / (1024f * 1024f); // MB로 변환
                    await ProgressForDownloadAssets(label);
                }
                else // 이미 다 다운 받은 경우
                {
                    Utils.Log("이미 모든 애셋이 다운로드됨!");
                }
                isInitDownload = true;
                downloadUI?.gameObject.SetActive(false);
            }
        }
        catch (Exception e)
        {
            Utils.Log($"다운로드 크기 확인 중 오류 발생: {e.Message}");
        }
    }

    /// <summary>
    /// Addressable 에셋을 미리 다운로드 받아야 LoadAssetAsync 시 다운받는 딜레이 없이 사용 가능
    /// 에셋 실 다운로드
    /// </summary>
    /// <param name="label">다운받을 에셋의 라벨</param>
    /// <returns></returns>
    private async UniTask ProgressForDownloadAssets(string label)
    {
        // label에 대해 에셋 번들 다운로드
        var downloadHandle = Addressables.DownloadDependenciesAsync(label);
        try
        {
            // 다운로드가 끝날 때 까지 대기
            while (!downloadHandle.IsDone)
            {
                // 다운로드 진행률:progress로 표시
                downloadUI?.SetProgress(downloadHandle.PercentComplete, allDownloadSize);
                await UniTask.Yield();
            }

            if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                // 다운로드 완료 시
                Utils.Log("다운로드 완료!");
            }
            else
            {
                // 다운로드 실패 시
                Utils.Log("다운로드 실패!");
            }
        }
        catch (Exception e)
        {
            Utils.Log($"다운로드 중 오류 발생: {e.Message}");
        }
        finally
        {
            Addressables.Release(downloadHandle); // 리소스에 대한 참조를 메모리가 해제하기
        }
    }

}
