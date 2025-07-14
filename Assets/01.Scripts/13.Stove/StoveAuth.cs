using Cysharp.Threading.Tasks;
using UnityEngine;

public class StoveAuth : MonoBehaviour
{
    public async UniTask SignInAsync()
    {
        await UniTask.WaitUntil(() => StoveManager.Instance.User.MemberNo > 0);
        Utils.Log($"로그인 완료: {StoveManager.Instance.User.Nickname}");
    }
}
