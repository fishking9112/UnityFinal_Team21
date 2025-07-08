using Cysharp.Threading.Tasks;
using Stove.PCSDK.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using UnityEditor.AddressableAssets.GUI;
using UnityEngine;

public class StoveGameServiceManager : MonoSingleton<StoveGameServiceManager>
{
    public StoveAuth Auth { get; private set; }
    public StoveSaveLoad SaveLoad { get; private set; }
    public StoveLeaderboard Leaderboard { get; private set; }
    public StoveAchievement Achievement { get; private set; }

    public TextMeshProUGUI UIDtext;
    public event System.Action OnRequireNickname;

    private UniTaskCompletionSource<bool> nicknameRegisterTCS;


    protected override void Awake()
    {
        base.Awake();

        Auth = GetComponentInChildren<StoveAuth>();
        SaveLoad = GetComponentInChildren<StoveSaveLoad>();
        Leaderboard = GetComponentInChildren<StoveLeaderboard>();
        Achievement = GetComponentInChildren<StoveAchievement>();

        if (Achievement == null || SaveLoad == null || Leaderboard == null)
        {
            Utils.Log("Stove 관련 스크립트 누락");
        }
    }

    public async UniTask InitAsync()
    {
        UIDtext.text = "";
        await StartGameFlowAsync();
    }

    public async UniTask StartGameFlowAsync()
    {
        await Auth.SignInAsync();
        UIDtext.text = "STOVE 로그인됨";

        bool hasNickname = await Auth.HasNicknameAsync();

        if (!hasNickname)
        {
            nicknameRegisterTCS = new UniTaskCompletionSource<bool>();

            SceneLoadManager.Instance.titleProgressText.ActiveUIGroup(false);

            OnRequireNickname?.Invoke();

            await nicknameRegisterTCS.Task;

            SceneLoadManager.Instance.titleProgressText.ActiveUIGroup(true);
        }

        await LoadPlayerDataAsync();
    }

    public void CompleteNicknameRegistration()
    {
        nicknameRegisterTCS?.TrySetResult(true);
    }

    public async UniTask LoadPlayerDataAsync()
    {
       // await SaveLoad.LoadAsync();
    }

    public async UniTask UploadScoreAsync(int score)
    {
        await Leaderboard.UploadScoreAsync(score);
    }

    public async UniTask LoadLeaderboardTop10Async()
    {
        await Leaderboard.GetTop10ScoresAsync();
    }

    public async UniTask LoadMyRankAsync()
    {
        await Leaderboard.GetMyRankAsync();
    }
}
