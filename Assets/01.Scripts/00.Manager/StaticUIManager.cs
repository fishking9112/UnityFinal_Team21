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


    public async UniTask LoadUI(LoadSceneEnum sceneEnum)
    {
        await hudLayer.LoadHUD(sceneEnum);
    }
}
