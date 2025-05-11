using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

public class Buff
{
    public int id;
    public int level;
    public CancellationTokenSource token;
    public BuffParticleController particleController;

    public Buff(int id, int level, CancellationTokenSource token, BuffParticleController particleController = null)
    {
        this.id = id;
        this.level = level;
        this.token = token;
        this.particleController = particleController;
    }

    public void UpdateToken()
    {
        token?.Cancel();
        token?.Dispose();
        token = new CancellationTokenSource();
    }
}

public class BuffManager : MonoSingleton<BuffManager>
{
    private Dictionary<int, BuffInfo> buffDic;
    private Dictionary<int, IBuffStrategy> buffStrategyDic;

    private void Start()
    {
        buffDic = DataManager.Instance.buffDic;
        buffStrategyDic = new Dictionary<int, IBuffStrategy>();

        InitBuffStrategyDic();
    }

    private void InitBuffStrategyDic()
    {
        buffStrategyDic[(int)IDBuff.ATTACK_DAMAGE_UP] = new AttackDamageBuff();
        buffStrategyDic[(int)IDBuff.BURN] = new BurnBuff();
        buffStrategyDic[(int)IDBuff.POISON] = new PoisonBuff();
        buffStrategyDic[(int)IDBuff.SLOW] = new SlowBuff();
        buffStrategyDic[(int)IDBuff.DEATHSYBOL] = new DeathSybolBuff();
    }

    // 버프 아이디로 버프 가져오기
    private IBuffStrategy GetBuffStrategy(int id)
    {
        return buffStrategyDic.TryGetValue(id, out var strategy) ? strategy : null;
    }

    public async UniTask ApplyBuff(BaseController target, int id, int level, BuffParticleController particleController = null)
    {
        if (target == null || !buffDic.TryGetValue(id, out var buffInfo))
        {
            return;
        }

        // 버프 적용
        Buff buff = AddBuff(target, buffInfo, level, particleController);
        await ApplyBuffDurationTime(target, buffInfo, buff.token, particleController);
    }

    // 버프 지속 시간 적용
    private async UniTask ApplyBuffDurationTime(BaseController target, BuffInfo info, CancellationTokenSource token, BuffParticleController particleController = null)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(info.durationTime), false, PlayerLoopTiming.Update, token.Token);

            // 버프 해제
            if (target != null)
            {
                RemoveBuff(target, info);

                if (particleController != null)
                {
                    particleController.RemoveParticle();
                }
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

    // 버프 추가
    private Buff AddBuff(BaseController target, BuffInfo info, int level, BuffParticleController particleController = null)
    {
        CancellationTokenSource token = new CancellationTokenSource();
        Buff buff = target.buffController.AddBuff(info.id, level, token);
        buff.particleController = particleController;

        float amount = GetAmountByLevel(info, level);
        var buffStrategy = GetBuffStrategy(info.id);
        buffStrategy?.Apply(target, buff, info, amount);

        return buff;
    }

    // 버프 제거
    private void RemoveBuff(BaseController target, BuffInfo info)
    {
        if (!target.buffController.buffDic.TryGetValue(info.id, out var buffList) || buffList.Count == 0)
        {
            return;
        }
        var buffStrategy = GetBuffStrategy(info.id);

        foreach (var buff in buffList)
        {
            if (buff == null)
            {
                continue;
            }

            buffStrategy?.Remove(target, buff, info);
            buff.particleController?.RemoveParticle();
        }
        target.buffController.RemoveBuff(info.id);
    }

    // 레벨에 따른 버프 수치를 가져오는 함수
    private float GetAmountByLevel(BuffInfo info, int level)
    {
        switch (level)
        {
            case 1:
                return info.lv_1;
            case 2:
                return info.lv_2;
            case 3:
                return info.lv_3;
            default:
                return 0f;
        }
    }
}
