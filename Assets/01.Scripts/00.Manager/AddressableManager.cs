using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourcePath
{
    public const string Audio = "Audio";
    public const string Datas = "Datas";
    public const string Images = "Images";
    public const string Prefabs = "Prefabs";
    public const string Thumbnail = "Thumbnail";
    public const string Scene = "Scene";
    public const string UI = "UI";
    public const string HUD = "HUD";
    public const string SINGLE = "Single";
}

/// <summary>
/// Addressable 에셋 관리
/// </summary>
public class AddressableManager : MonoSingleton<AddressableManager>
{
    private DownloadUI downloadUI => StaticUIManager.Instance.downloadUI;
    [HideInInspector] public bool isInitDownload = false;
    private float allDownloadSize = 0;

    protected override void Awake()
    {
        base.Awake();
    }

    public async UniTask InitDownloadAsync()
    {
        await DownloadAssets("InitDownload");
    }

    /// <summary>
    /// 에셋 동기 로드
    /// </summary>
    public T LoadAsset<T>(string key, string type) where T : UnityEngine.Object
    {
        var path = $"{type}/{key}";
        AsyncOperationHandle<T> handle = default;

        try
        {
            if (typeof(T).IsSubclassOf(typeof(MonoBehaviour)))
            {
                var goHandle = Addressables.LoadAssetAsync<GameObject>(path);
                var gameObject = goHandle.WaitForCompletion();
                return gameObject.GetComponent<T>();
            }
            else
            {
                handle = Addressables.LoadAssetAsync<T>(path);
                var result = handle.WaitForCompletion();
                return result;
            }
        }
        catch (Exception e)
        {
            Utils.Log($"로드 중 오류 발생: {path}, {e.Message}");
            return default;
        }
        finally
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }
    }

    /// <summary>
    /// 라벨로 에셋 리스트 동기 로드
    /// </summary>
    public List<T> LoadDataAssets<T>(string label)
    {
        try
        {
            var list = Addressables.LoadAssetsAsync<T>(label, (obj) =>
            {
                Debug.Log(obj.ToString());
            }).WaitForCompletion();
            List<T> assets = list != null ? new List<T>(list) : new List<T>();
            return assets;
        }
        catch (Exception e)
        {
            Utils.Log($"로드 중 오류 발생: {label}, {e.Message}");
            return default;
        }
    }
    /// <summary>
    /// 에셋 비동기 로드
    /// </summary>
    public async UniTask<T> LoadAssetAsync<T>(string key, string type) where T : UnityEngine.Object
    {
        var path = $"{type}/{key}";
        AsyncOperationHandle<T> handle = default;

        try
        {
            if (typeof(T).IsSubclassOf(typeof(MonoBehaviour)))
            {
                var goHandle = Addressables.LoadAssetAsync<GameObject>(path);
                var gameObject = await goHandle.ToUniTask();
                return gameObject.GetComponent<T>();
            }
            else
            {
                handle = Addressables.LoadAssetAsync<T>(path);
                var result = await handle.ToUniTask();
                return result;
            }
        }
        catch (Exception e)
        {
            Utils.Log($"로드 중 오류 발생: {path}, {e.Message}");
            return null;
        }
        finally
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }
    }

    /// <summary>
    /// 라벨로 에셋 리스트 비동기 로드
    /// </summary>
    public async UniTask<List<T>> LoadDataAssetsAsync<T>(string label)
    {
        AsyncOperationHandle<IList<T>> handle = default;
        try
        {
            handle = Addressables.LoadAssetsAsync<T>(label, null);
            var result = await handle.ToUniTask();
            return new List<T>(result);
        }
        catch (Exception e)
        {
            Utils.Log($"로드 중 오류 발생: {label}, {e.Message}");
            return null;
        }
        finally
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }
    }

    /// <summary>
    /// 에셋 다운로드
    /// </summary>
    private async UniTask DownloadAssets(string label)
    {
        if (downloadUI == null)
        {
            Utils.LogError("DownloadUI is not assigned!");
            return;
        }

        try
        {
            var sizeHandle = Addressables.GetDownloadSizeAsync(label);
            await sizeHandle.ToUniTask();

            if (sizeHandle.Status == AsyncOperationStatus.Succeeded)
            {
                downloadUI.gameObject.SetActive(true);
                if (sizeHandle.Result > 0)
                {
                    allDownloadSize = sizeHandle.Result / (1024f * 1024f); // MB
                    await ProgressForDownloadAssets(label);
                }
                else
                {
                    Utils.Log("이미 모든 에셋이 다운로드됨!");
                }
                isInitDownload = true;
            }
        }
        catch (Exception e)
        {
            Utils.Log($"다운로드 크기 확인 중 오류 발생: {e.Message}");
        }
        finally
        {
            downloadUI.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 에셋 실제 다운로드 및 진행률 업데이트
    /// </summary>
    private async UniTask ProgressForDownloadAssets(string label)
    {
        var downloadHandle = Addressables.DownloadDependenciesAsync(label);
        float lastProgress = -1f;

        try
        {
            while (!downloadHandle.IsDone)
            {
                float progress = downloadHandle.PercentComplete;
                if (Mathf.Abs(progress - lastProgress) > 0.01f) // 1% 이상 변경 시 UI 업데이트
                {
                    downloadUI.SetProgress(progress, allDownloadSize);
                    lastProgress = progress;
                }
                await UniTask.Yield();
            }

            if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Utils.Log("다운로드 완료!");
            }
            else
            {
                Utils.Log("다운로드 실패!");
            }
        }
        catch (Exception e)
        {
            Utils.Log($"다운로드 중 오류 발생: {e.Message}");
        }
        finally
        {
            Addressables.Release(downloadHandle);
        }
    }

    /// <summary>
    /// 비동기적으로 개별 에셋 언로드
    /// </summary>
    public async UniTask UnloadAssetAsync<T>(T asset) where T : UnityEngine.Object
    {
        if (asset == null) return;

        try
        {
            // 비동기적으로 에셋 언로드
            await UniTask.RunOnThreadPool(() => Addressables.Release(asset));
            Utils.Log($"에셋 언로드 완료: {asset.name}");
        }
        catch (Exception e)
        {
            Utils.Log($"에셋 언로드 중 오류 발생: {e.Message}");
        }
    }

    /// <summary>
    /// 비동기적으로 여러 에셋 언로드
    /// </summary>
    public async UniTask UnloadAssetsAsync<T>(List<T> assets) where T : UnityEngine.Object
    {
        foreach (var asset in assets)
        {
            await UnloadAssetAsync(asset);
        }
    }

    /// <summary>
    /// 라벨로 비동기적으로 에셋 리스트 언로드
    /// </summary>
    public async UniTask UnloadAssetsByLabelAsync(string label)
    {
        try
        {
            var handle = Addressables.LoadAssetsAsync<UnityEngine.Object>(label, null);
            var assets = await handle.ToUniTask();

            foreach (var asset in assets)
            {
                await UnloadAssetAsync(asset); // 비동기적으로 언로드
            }

            Utils.Log($"라벨로 에셋 언로드 완료: {label}");
        }
        catch (Exception e)
        {
            Utils.Log($"라벨로 에셋 언로드 중 오류 발생: {e.Message}");
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        // 추가 정리 작업이 필요하면 여기에 구현
    }
}