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

    public async UniTask ApplyBuff(MonsterController target, int id, int level)
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
            // 현재 적용되어 있는 버프가 지금 적용하려는 버프보다 레벨이 높은 경우 무시
            if (curBuffLevel > level)
            {
                return;
            }
            else if (curBuffLevel == level)
            {
                DurationUpdate(target, buffInfo);
                return;
            }
            else
            {
                // 이전에 적용되어 있는 동일 버프 제거
                RemoveBuff(target, buffInfo);
            }
        }

        // 버프 적용
        CancellationTokenSource token = AddBuff(target, buffInfo, level);

        await ApplyBuffDurationTime(target, buffInfo, token);
    }

    private async UniTask ApplyBuffDurationTime(MonsterController target, BuffInfo info, CancellationTokenSource token)
    {
        try
        {
            // 버프 지속 시간
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

    private void DurationUpdate(MonsterController target, BuffInfo info)
    {
        target.RemoveBuffToken(info.id);

        CancellationTokenSource token = new CancellationTokenSource();
        target.AddBuffToken(info.id, token);

        ApplyBuffDurationTime(target, info, token).Forget();
    }

    // 버프 추가
    private CancellationTokenSource AddBuff(MonsterController target, BuffInfo info, int level)
    {
        CancellationTokenSource token = new CancellationTokenSource();

        target.AddBuffToken(info.id, token);
        target.buffDic[info.id] = level;

        //switch(info.type)
        //target.UpgradeAttack(GetAmountByLevel(info, level));

        return token;
    }

    // 버프 제거
    private void RemoveBuff(MonsterController target, BuffInfo info)
    {
        if (target.buffDic.TryGetValue(info.id, out int level))
        {
            target.RemoveBuffToken(info.id);
            target.buffDic.Remove(info.id);

            //switch(info.type)
            //target.UpgradeAttack(-GetAmountByLevel(info, level));
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
