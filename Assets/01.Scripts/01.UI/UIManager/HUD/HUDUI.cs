using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class HUDUI : MonoBehaviour
{
    public virtual async UniTask Initialize()
    {
        await UniTask.Yield(PlayerLoopTiming.Update);
    }
}
