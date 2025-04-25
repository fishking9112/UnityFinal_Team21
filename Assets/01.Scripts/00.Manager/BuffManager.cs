using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;

public class BuffManager : MonoSingleton<BuffManager>
{
    private Dictionary<int, BuffInfo> buffDic;

    private void Start()
    {
        buffDic = DataManager.Instance.buffDic;
    }

    public async UniTask ApplyBuff(BaseController target, int id, int level)
    {
        if (target == null)
        {
            return;
        }
        if (!buffDic.TryGetValue(id, out var buffInfo))
        {
            return;
        }

        if (target.buffDic.TryGetValue(id, out int curBuffLevel))
        {
            if (curBuffLevel > level)
            {
                // 현재 적용되어 있는 버프가 지금 적용하려는 버프보다 레벨이 높은 경우 무시
                return;
            }
            else if (curBuffLevel == level)
            {
                // 현재 적용되어 있는 버프가 지금 적용하려는 버프랑 레벨이 같을 경우 시간만 갱신(새로운 토큰으로 변경)
                target.RemoveBuffToken(id, true);
                var updateToken = new CancellationTokenSource();
                target.AddBuffToken(id, updateToken);

                _ = ApplyBuffDurationTime(target, buffInfo, updateToken);
                return;
            }
            else
            {
                // 이전에 적용되어 있는 동일 버프 제거
                RemoveBuff(target, buffInfo);
            }
        }

        // 버프 적용(최초 적용 or 더높은 레벨의 동일 버프가 들어올 때 적용)
        CancellationTokenSource token = AddBuff(target, buffInfo, level);
        await ApplyBuffDurationTime(target, buffInfo, token);
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
    }

    // 버프 추가
    private CancellationTokenSource AddBuff(BaseController target, BuffInfo info, int level)
    {
        CancellationTokenSource token = new CancellationTokenSource();

        target.AddBuffToken(info.id, token);
        target.buffDic[info.id] = level;

        float amount = GetAmountByLevel(info, level);

        switch (info.type)
        {
            case BuffType.ATTACK_DMG:
                target.UpgradeAttack(amount);
                break;
            case BuffType.ATTACK_SPEED:
                target.UpgradeAttackSpeed(amount);
                break;
            case BuffType.MOVE_SPEED:
                target.UpgradeMoveSpeed(amount);
                break;
            case BuffType.POISON:
                break;
            case BuffType.BURN:
                _ = TakeTickDamaged(target, info, token, level);
                break;
        }

        return token;
    }

    // 버프 제거
    public void RemoveBuff(BaseController target, BuffInfo info)
    {
        if (target.buffDic.TryGetValue(info.id, out int level))
        {
            target.RemoveBuffToken(info.id);
            target.buffDic.Remove(info.id);

            switch (info.type)
            {
                case BuffType.ATTACK_DMG:
                    target.UpgradeAttack(-GetAmountByLevel(info, level));
                    break;
                case BuffType.ATTACK_SPEED:
                    target.UpgradeAttackSpeed(-GetAmountByLevel(info, level));
                    break;
                case BuffType.MOVE_SPEED:
                    target.UpgradeMoveSpeed(-GetAmountByLevel(info, level));
                    break;
                case BuffType.POISON:
                    break;
                case BuffType.BURN:
                    break;
            }
        }
    }

    private async UniTask TakeTickDamaged(BaseController target, BuffInfo info, CancellationTokenSource token, int level)
    {
        int tickCount = (int)MathF.Floor(info.durationTime / info.tick);
        float tickDamage = GetAmountByLevel(info, level);

        try
        {
            for (int i = 0; i < tickCount; i++)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(info.tick), false, PlayerLoopTiming.Update, token.Token);
                if (target == null || !target.buffDic.ContainsKey(info.id))
                {
                    break;
                }
                target.TakeDamaged(tickDamage);
            }
        }
        catch (OperationCanceledException)
        {

        }
    }

    // 레벨에 따른 버프 수치를 가져오는 함수
    private float GetAmountByLevel(BuffInfo info, int level)
    {
        float amount;

        switch (level)
        {
            case 1:
                amount = info.lv_1;
                break;
            case 2:
                amount = info.lv_2;
                break;
            case 3:
                amount = info.lv_3;
                break;
            default:
                amount = 0;
                break;
        }
        return amount;
    }
}
