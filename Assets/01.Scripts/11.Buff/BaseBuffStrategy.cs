using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;

public class BaseBuffStrategy
{
    // 틱 데미지
    protected async UniTask TakeTickDamaged(BaseController target, BuffInfo info, CancellationTokenSource token, int level, float amount)
    {
        int tickCount = (int)(info.durationTime / info.tick);
        float tickDamage = amount;

        try
        {
            for (int i = 0; i < tickCount; i++)
            {
                if (target == null || !target.buffController.buffDic.ContainsKey(info.id))
                {
                    break;
                }
                target.TakeDamaged(tickDamage);

                await UniTask.Delay(TimeSpan.FromSeconds(info.tick), false, PlayerLoopTiming.Update, token.Token);
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

    // 파티클 제거
    protected void RemoveParticle(Buff buff)
    {
        if (buff.particle != null)
        {
            buff.particle.OnDespawn();
            buff.particle = null;
        }
    }
}
