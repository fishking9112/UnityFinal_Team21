using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

public class Buff
{
    public int id;
    public int level;
    public CancellationTokenSource token;
    public ParticleObject particle;

    public Buff(int id, int level, CancellationTokenSource token)
    {
        this.id = id;
        this.level = level;
        this.token = token;
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

    #region InitBuffStraegyDic
    private void InitBuffStrategyDic()
    {
        buffStrategyDic[(int)IDBuff.ATTACK_DAMAGE_UP] = new AttackDamageBuff();
        buffStrategyDic[(int)IDBuff.BURN] = new BurnBuff();
        buffStrategyDic[(int)IDBuff.POISON] = new PoisonBuff();
        buffStrategyDic[(int)IDBuff.SLOW] = new SlowBuff();
        buffStrategyDic[(int)IDBuff.DEATHSYMBOL] = new DeathSymbolBuff();
        buffStrategyDic[(int)IDBuff.MILITIA] = new MilitiaBuff();
        buffStrategyDic[(int)IDBuff.ATTACK_DAMAGE_AND_ATTACK_SPEED_UP] = new AttackDamageAndAttackSpeedBuff();
        buffStrategyDic[(int)IDBuff.ATTACK_DAMAGE_AND_MOVE_SPEED_AND_ATTACK_SPEED_UP] = new AttackDamageAndMoveSpeedAndAttackSpeedBuff();
        buffStrategyDic[(int)IDBuff.GIANT_FORM] = new GiantFormBuff();
        buffStrategyDic[(int)IDBuff.DECAY] = new DecayBuff();
    }
    #endregion

    // 버프 아이디로 버프 가져오기
    public IBuffStrategy GetBuffStrategy(int id)
    {
        return buffStrategyDic.TryGetValue(id, out var strategy) ? strategy : null;
    }

    public async UniTask ApplyBuff(BaseController target, int id, int level)
    {
        if (target == null || !buffDic.TryGetValue(id, out var buffInfo))
        {
            return;
        }

        // 버프 적용 (이미 버프가 걸려있는 지는 중요하지 않음)
        Buff buff = AddBuff(target, buffInfo, level);
        await ApplyBuffDurationTime(target, buffInfo, buff.token);

    }

    // 버프 지속 시간 적용
    private async UniTask ApplyBuffDurationTime(BaseController target, BuffInfo info, CancellationTokenSource token)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(info.durationTime), false, PlayerLoopTiming.Update, token.Token);

            // 버프 해제
            if (target != null)
            {
                RemoveBuff(target, info);
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
    private Buff AddBuff(BaseController target, BuffInfo info, int level)
    {
        CancellationTokenSource token = new CancellationTokenSource();
        Buff buff = target.buffController.AddBuff(info.id, level, token);

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

            buffStrategy?.Remove(target, buff);
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
