using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class StaticUIManager : MonoSingleton<StaticUIManager>
{
    public HUDLayer hudLayer;
    public DamageLayer damageLayer;


    public LoadingUI loadingUI;
    public DownloadUI downloadUI;


    public async UniTask LoadUI(string sceneName)
    {
        switch (sceneName)
        {
            case "LoginScene": // 로그인 씬 일 경우
                break;
            case "MenuScene": // 메뉴 씬 일 경우
                await hudLayer.LoadHUD(sceneName);
                break;
            case "GameScene": // 게임 씬 일 경우
                await hudLayer.LoadHUD(sceneName);
                break;
            default:
                // await LoadSceneAsync("Error"); // 에러씬으로 이동(?)
                break;
        }
    }
}
