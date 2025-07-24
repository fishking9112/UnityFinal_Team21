using Cysharp.Threading.Tasks;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using Unity.Services.Core;
using UnityEngine;

public class UGSAuth : MonoBehaviour
{
    private const string NicknameKey = "NickName";

    private Callback<GetTicketForWebApiResponse_t> m_AuthTicketForWebApiResponseCallback;
    private string m_SessionTicket;
    private const string steamIdentity = "unityauthenticationservice";

    private TaskCompletionSource<bool> _steamLoginTCS;

    public async UniTask SignInWithSteamAsync()
    {
        // UGS 초기화
        await UnityServices.InitializeAsync();

        if (!SteamManager.Initialized)
        {
            Utils.LogError("SteamManager가 초기화되지 않았습니다.");
            return;
        }

        _steamLoginTCS = new TaskCompletionSource<bool>();

        // Steam 세션 티켓 요청
        m_AuthTicketForWebApiResponseCallback = Callback<GetTicketForWebApiResponse_t>.Create(OnAuthCallback);
        SteamUser.GetAuthTicketForWebApi(steamIdentity);

        // 로그인 완료될 때까지 대기
        await _steamLoginTCS.Task;
    }

    private async void OnAuthCallback(GetTicketForWebApiResponse_t callback)
    {
        // 콜백에서 세션 티켓을 16진수 문자열로 받음
        m_SessionTicket = BitConverter.ToString(callback.m_rgubTicket, 0, (int)callback.m_cubTicket).Replace("-", string.Empty);

        Utils.Log($"Steam 세션 티켓 획득 완료: {m_SessionTicket}");

        m_AuthTicketForWebApiResponseCallback.Dispose();
        m_AuthTicketForWebApiResponseCallback = null;

        try
        {
            await AuthenticationService.Instance.SignInWithSteamAsync(m_SessionTicket, steamIdentity);
            Utils.Log($"UGS Steam 로그인 성공 - PlayerID: {AuthenticationService.Instance.PlayerId}");
            _steamLoginTCS?.SetResult(true);
        }
        catch (AuthenticationException e)
        {
            Utils.LogError($"UGS 인증 실패: {e.Message}");
            _steamLoginTCS?.SetException(e);
        }
        catch (RequestFailedException e)
        {
            Utils.LogError($"UGS 요청 실패: {e.Message}");
            _steamLoginTCS?.SetException(e);
        }
    }


    /// <summary>
    /// 익명 로그인 (게스트 로그인)
    /// </summary>
    public async UniTask SignInAnonymously()
    {
        if (UGSManager.Instance.IsLoggedIn)
        {
            Utils.Log("이미 로그인되어 있음");
            return;
        }

        // Unity Services 초기화 보장
        if (UnityServices.State != ServicesInitializationState.Initialized)
            await UnityServices.InitializeAsync();

        try
        {
            // 시도 1: 최초 로그인 시도
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Utils.Log($"1차 로그인 성공: {UGSManager.Instance.PlayerId}");
        }
        catch (AuthenticationException authEx)
        {
            // 로그인 실패: 세션 토큰 문제일 수 있음
            Utils.LogWarning($"1차 로그인 실패: {authEx.Message}");

            // 세션 토큰이 존재하지만 유효하지 않음 → 로그아웃
            if (AuthenticationService.Instance.SessionTokenExists)
            {
                Utils.Log("무효한 세션 토큰 존재 → 로그아웃 후 재시도");
                AuthenticationService.Instance.SignOut();

                try
                {
                    // 시도 2: 다시 로그인
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Utils.Log($"2차 로그인 성공(세션 탈출): {UGSManager.Instance.PlayerId}");
                }
                catch (Exception secondEx)
                {
                    Utils.LogError($"2차 로그인도 실패(세션 탈출): {secondEx.Message}");
                }
            }

            Utils.LogWarning($"2차 로그인 시도(세션클리어): {authEx.Message}");
            AuthenticationService.Instance.ClearSessionToken();
            try
            {
                // 시도 2: 다시 로그인
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Utils.Log($"2차 로그인 성공(세션클리어): {UGSManager.Instance.PlayerId}");
            }
            catch (Exception secondEx)
            {
                Utils.LogError($"2차 로그인도 실패(세션클리어): {secondEx.Message}");
            }
        }

    }


    /// <summary>
    /// 닉네임 존재 여부 확인
    /// </summary>
    public async UniTask<bool> HasNicknameAsync()
    {
        var data = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { NicknameKey }, new LoadOptions(new PublicReadAccessClassOptions(UGSManager.Instance.PlayerId)));
        return data.ContainsKey(NicknameKey);
    }

    /// <summary>
    /// 닉네임 저장
    /// </summary>
    public async UniTask SaveNicknameAsync(string nickname)
    {
        var data = new Dictionary<string, object>
        {
            { NicknameKey, nickname }
        };
        await CloudSaveService.Instance.Data.Player.SaveAsync(data, new Unity.Services.CloudSave.Models.Data.Player.SaveOptions(new PublicWriteAccessClassOptions()));
    }

    /// <summary>
    /// 플레이어ID 기반의 닉네임 가져오기
    /// </summary>
    public async UniTask<string> LoadPublicDataByPlayerId(string playerId)
    {
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> { NicknameKey }, new LoadOptions(new PublicReadAccessClassOptions(playerId)));

        return playerData.TryGetValue(NicknameKey, out var nickname) ? nickname.Value.GetAsString() : "Unknown";
    }
}
