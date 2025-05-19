using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class DeathSymbolBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        buff.particle = ParticleManager.Instance.SpawnParticle("DeathSymbol_Buff", target.transform.position + Vector3.up, new Vector3(0.2f, 0.2f, 2f), Quaternion.identity, target.transform);
        _ = DeathSymbol(target, info.durationTime);
    }

    public void Remove(BaseController target, Buff buff)
    {
        RemoveParticle(buff);
    }

    protected async UniTask DeathSymbol(BaseController target, float delay)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), false, PlayerLoopTiming.Update);

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
