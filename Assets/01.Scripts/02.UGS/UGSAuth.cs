using Cysharp.Threading.Tasks;
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
