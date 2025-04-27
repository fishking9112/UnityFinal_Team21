using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ! 아직 안사용함 !
/// </summary>
public class SingleLayer : MonoBehaviour
{
    private Dictionary<string, SingleUI> uiInstances = new();

    public async UniTask LoadSingle(string sceneName)
    {
        await UnloadCurrentUI();

        switch (sceneName)
        {
            case "LoginScene":
                break;
            case "MenuScene":
                // await LoadCurrentUI("MenuHUD");
                break;
            case "GameScene":
                await LoadCurrentUI("EvolutionTreeUI");
                await LoadCurrentUI("QueenEnhanceUI");
                await LoadCurrentUI("PauseUI");
                await LoadCurrentUI("ResultUI");
                break;
            default:
                break;
        }
    }

    public async UniTask<SingleUI> LoadCurrentUI(string uiName)
    {
        if (uiInstances.ContainsKey(uiName))
            return uiInstances[uiName];

        var uiPrefab = await AddressableManager.Instance.LoadAssetAsync<SingleUI>($"{uiName}.prefab", ResourcePath.SINGLE);
        if (uiPrefab != null)
        {
            var instance = Instantiate(uiPrefab, this.transform);
            instance.Initialize();
            instance.gameObject.SetActive(false); // 처음에는 꺼두자
            uiInstances.Add(uiName, instance);
            return instance;
        }
        else
        {
            Utils.LogError($"HUD 로드 실패: {ResourcePath.SINGLE}/{uiName}.prefab");
            return null;
        }
    }

    public async UniTask UnloadCurrentUI()
    {
        foreach (var pair in uiInstances)
        {
            if (pair.Value != null)
            {
                Destroy(pair.Value.gameObject);
            }
        }
        uiInstances.Clear();

        await UniTask.Yield();
    }

    public T GetUI<T>(string uiName) where T : SingleUI
    {
        if (uiInstances.TryGetValue(uiName, out var ui))
        {
            return ui as T;
        }
        return null;
    }

    public T ShowUI<T>(string uiName) where T : SingleUI
    {
        // 먼저 모든 UI 끄기
        foreach (var ui in uiInstances.Values)
        {
            if (ui != null)
            {
                ui.gameObject.SetActive(false);
            }
        }

        // 원하는 UI 켜기
        if (uiInstances.TryGetValue(uiName, out var targetUI))
        {
            targetUI.gameObject.SetActive(true);
            return targetUI as T;
        }

        Utils.LogWarning($"ShowUI 실패: {uiName} 없음");
        return null;
    }

    public void AllHideUI()
    {
        foreach (var ui in uiInstances.Values)
        {
            if (ui != null)
            {
                ui.gameObject.SetActive(false);
            }
        }
    }
}
