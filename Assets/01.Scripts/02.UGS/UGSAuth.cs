using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;

public class UGSAuth : MonoBehaviour
{
    public bool IsLoggedIn => AuthenticationService.Instance.IsSignedIn;
    public string PlayerId => AuthenticationService.Instance.PlayerId;

    private async void Start()
    {
        await InitializeUGS();
        await SignInAnonymously();
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
            Debug.Log("UGS 초기화 완료");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"UGS 초기화 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 익명 로그인 (게스트 로그인)
    /// </summary>
    public async Task SignInAnonymously()
    {
        if (IsLoggedIn)
        {
            Debug.Log("이미 로그인되어 있음");
            return;
        }

        try
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"익명 로그인 성공 Player ID: {PlayerId}");
        }
        catch (AuthenticationException e)
        {
            Debug.LogError($"로그인 실패 {e.Message}");
        }
        catch (RequestFailedException e)
        {
            Debug.LogError($"로그인 요청 실패 {e.Message}");
        }
    }
}
}
