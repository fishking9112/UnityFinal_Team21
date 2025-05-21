using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class HUDLayer : MonoBehaviour
{
    [NonSerialized] private HUDUI currentHUD; // 인스턴스화된 HUD (HUDUI를 부모로 둔 자식 인스턴스)

    public async UniTask LoadHUD(LoadSceneEnum sceneEnum)
    {
        // 활성화 되어 있던 HUD 반납
        await UnloadCurrentHUD();

        switch (sceneEnum)
        {
            case LoadSceneEnum.AppScene: // 로그인 씬 일 경우
                break;
            case LoadSceneEnum.MenuScene: // 메뉴 씬 일 경우
                MenuHUD menuHUD = await LoadCurrentHUD("MenuHUD") as MenuHUD;
                // menuHUD.Setup();

                break;
            case LoadSceneEnum.GameScene: // 게임 씬 일 경우
                GameHUD gameHUD = await LoadCurrentHUD("GameHUD") as GameHUD;

                break;
            default:
                break;
        }
    }

    public async UniTask<HUDUI> LoadCurrentHUD(string hudName)
    {
        var hudPrefab = await AddressableManager.Instance.LoadAssetAsync<HUDUI>($"{hudName}.prefab", ResourcePath.HUD);
        if (hudPrefab != null)
        {
            currentHUD = Instantiate(hudPrefab, this.transform);
            await currentHUD.Initialize();
        }
        else
        {
            Utils.LogError($"HUD 로드 실패: {ResourcePath.HUD}/{hudName}.prefab");
        }

        return hudPrefab;
    }

    public async UniTask UnloadCurrentHUD()
    {
        if (currentHUD != null)
        {
            Destroy(currentHUD.gameObject);
            currentHUD = null;
        }

        await UniTask.Yield();
    }

    public T GetHUD<T>() where T : HUDUI
    {
        return currentHUD as T;
    }
}