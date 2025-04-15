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

    /// <summary>
    /// 게임 시작 시 호출되는 초기 흐름 처리 메서드입니다.
    /// 익명 로그인 시도 후, 닉네임 유무를 확인합니다.
    /// 닉네임이 없을 경우 외부에서 UI를 띄울 수 있도록 이벤트를 호출합니다.
    /// </summary>
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

        await LoadPlayerDataAsync();
        SceneLoadManager.Instance.LoadScene("MainUITest").Forget();
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

    /// <summary>
    /// 플레이어 데이터 로드
    /// </summary>
    public async UniTask LoadPlayerDataAsync()
    {
        await SaveLoad.LoadAsync();
    }

    /// <summary>
    /// 리더보드에 플레이어 점수를 업로드합니다.
    /// </summary>
    /// <param name="score">업로드할 점수</param>
    public async UniTask UploadScoreAsync(int score)
    {
        await Leaderboard.UploadScoreAsync(score);
    }

    /// <summary>
    /// 리더보드에서 Top 10 플레이어의 점수를 불러옵니다.
    /// </summary>
    public async UniTask LoadLeaderboardTop10Async()
    {
        await Leaderboard.GetTop10ScoresAsync();
    }

    /// <summary>
    /// 현재 플레이어의 리더보드 순위 및 점수를 불러옵니다.
    /// </summary>
    public async UniTask LoadMyRankAsync()
    {
        await Leaderboard.GetMyRankAsync();
    }
}
