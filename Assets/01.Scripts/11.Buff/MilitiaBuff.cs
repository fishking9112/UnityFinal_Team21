using Cysharp.Threading.Tasks;
using System;

public class MilitiaBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        _ = Militia(target, info.durationTime);
    }

    public void Remove(BaseController target, Buff buff, BuffInfo info)
    {

    }

    protected async UniTask Militia(BaseController target, float delay)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));

            if (target != null)
            {
                target.Die();
            }
        }
        catch (OperationCanceledException)
        {
            // 버프가 중간에 끊겼을 때 예외. 무시해도 됨
        }
        catch (ObjectDisposedException)
        {
            // 버프가 중간에 끊겼을 때 예외. 무시해도 됨
        }
    }
}
