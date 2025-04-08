using Cysharp.Threading.Tasks;
using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class UGSManager : MonoSingleton<UGSManager>
{
    public UGSAuth Auth { get; private set; }
    public UGSSaveLoad SaveLoad { get; private set; }
    public UGSLeaderboard Leaderboard { get; private set; }

    public bool IsLoggedIn => AuthenticationService.Instance.IsSignedIn;
    public string PlayerId => AuthenticationService.Instance.PlayerId;

    public event Action OnRequireNickname;

    protected override void Awake()
    {
        base.Awake();

        Auth = GetComponentInChildren<UGSAuth>();
        SaveLoad = GetComponentInChildren<UGSSaveLoad>();
        Leaderboard = GetComponentInChildren<UGSLeaderboard>();

        if (Auth == null || SaveLoad == null || Leaderboard == null)
        {
            Utils.Log("UGS 관련 스크립트 누락");
        }

    }
    private async void Start()
    {
        await InitializeUGS();
        await StartGameFlowAsync();
    }

    public async UniTask StartGameFlowAsync()
    {
        if (!IsLoggedIn)
            await Auth.SignInAnonymously();

        bool hasNickname = await Auth.HasNicknameAsync();

        if (!hasNickname)
        {
            OnRequireNickname?.Invoke();  // 외부에서 UI 띄우도록 연결
            return;
        }

        await SaveLoad.LoadAsync();
        await Leaderboard.GetTop10ScoresAsync();
    }

    /// <summary>
    /// UGS 초기화
    /// </summary>
    private async Task InitializeUGS()
    {
        if (UnityServices.State == ServicesInitializationState.Initialized) return;

        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (System.Exception e)
        {
            Utils.Log($"UGS 초기화 실패: {e.Message}");
        }
    }

}
