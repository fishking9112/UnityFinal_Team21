using Cysharp.Threading.Tasks;
using UnityEngine;

public class StoveAuth : MonoBehaviour
{
    public async UniTask SignInAsync()
    {
        await UniTask.WaitUntil(() => StoveManager.Instance.User.MemberNo > 0);
        Utils.Log($"로그인 완료: {StoveManager.Instance.User.Nickname}");
    }

    public async UniTask<bool> HasNicknameAsync()
    {
        string nickname = StoveManager.Instance.User.Nickname;
        await UniTask.Yield();
        return !string.IsNullOrEmpty(nickname);
    }
}
