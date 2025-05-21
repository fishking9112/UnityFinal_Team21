using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.CloudSave.Models.Data.Player;
using Unity.Services.Core;
using UnityEngine;

public class UGSAuth : MonoBehaviour
{
    private const string NicknameKey = "NickName";


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

        /*
        if (UGSManager.Instance.IsLoggedIn)
        {
            Utils.Log("이미 로그인되어 있음");
            return;
        }

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Utils.Log($"익명 로그인 성공 Player ID: {UGSManager.Instance.PlayerId}");
        }
        catch (AuthenticationException e)
        {
            Utils.Log($"로그인 실패 {e.Message}");
        }
        catch (RequestFailedException e)
        {
            Utils.Log($"로그인 요청 실패 {e.Message}");
        }*/
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
