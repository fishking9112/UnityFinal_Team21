using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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

    public void UpdateToken(CancellationTokenSource newToken)
    {
        token?.Cancel();
        token?.Dispose();
        token = newToken;
    }
}

public class BuffManager : MonoSingleton<BuffManager>
{
    private Dictionary<int, BuffInfo> buffDic;

    private void Start()
    {
        buffDic = DataManager.Instance.buffDic;
    }

    public async UniTask ApplyBuff(BaseController target, int id, int level)
    {
        if (target == null || !buffDic.TryGetValue(id, out var buffInfo))
        {
            return;
        }

        if (!buffInfo.isStack)
        {
            if (target.buffDic.TryGetValue(id, out var buffList) && buffList.Count > 0)
            {
                var curBuffLevel = buffList[0].level;

                if (curBuffLevel > level)
                {
                    // 현재 적용되어 있는 버프가 지금 적용하려는 버프보다 레벨이 높은 경우 무시
                    return;
                }
                else if (curBuffLevel == level)
                {
                    // 현재 적용되어 있는 버프와 지금 적용하려는 버프의 레벨이 같은 경우 시간만 갱신(토큰 업데이트)
                    buffList[0].UpdateToken(new CancellationTokenSource());

                    await ApplyBuffDurationTime(target, buffInfo, buffList[0].token);
                    return;
                }
                else
                {
                    // 이전에 적용되어 있는 동일 버프 제거
                    RemoveBuff(target, buffInfo);
                }
            }

            // 버프 적용(최초 적용 or 더높은 레벨의 동일 버프가 들어올 때 적용)
            Buff buff = AddBuff(target, buffInfo, level);
            await ApplyBuffDurationTime(target, buffInfo, buff.token);
        }
        else
        {
            // 버프 적용 (이미 버프가 걸려있는 지는 중요하지 않음)
            Buff buff = AddBuff(target, buffInfo, level);
            await ApplyBuffDurationTime(target, buffInfo, buff.token);
        }
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
        Buff buff = target.AddBuff(info.id, level, token);

        float amount = GetAmountByLevel(info, level);

        switch (info.type)
        {
            case BuffType.ATTACK_DMG:
                buff.particle = ParticleManager.Instance.SpawnParticle("AttackDMG_Sword", target.transform.position + Vector3.up, Quaternion.identity, 0.5f, target.transform);
                target.AttackDamageBuff(amount);
                break;
            case BuffType.ATTACK_SPEED:
                target.AttackSpeedBuff(amount);
                break;
            case BuffType.MOVE_SPEED:
                target.MoveSpeedBuff(amount);
                break;
            case BuffType.POISON:
                break;
            case BuffType.BURN:
                buff.particle = ParticleManager.Instance.SpawnParticle("Burn", target.transform.position, Quaternion.identity, 0.5f, target.transform);
                _ = TakeTickDamaged(target, info, token, level);
                break;
        }

        return buff;
    }

    // 버프 제거
    public void RemoveBuff(BaseController target, BuffInfo info)
    {
        if (!target.buffDic.TryGetValue(info.id, out var buffList) || buffList.Count == 0)
        {
            return;
        }

        foreach (var buff in buffList)
        {
            if (buff == null)
            {
                continue;
            }

            float amount = GetAmountByLevel(info, buff.level);

            switch (info.type)
            {
                case BuffType.ATTACK_DMG:
                    RemoveParticle(buff);
                    target.EndAttackDamageBuff();
                    break;
                case BuffType.ATTACK_SPEED:
                    target.EndAttackSpeedBuff();
                    break;
                case BuffType.MOVE_SPEED:
                    target.EndMoveSpeedBuff();
                    break;
                case BuffType.POISON:
                    break;
                case BuffType.BURN:
                    RemoveParticle(buff);
                    break;
            }
        }
        target.RemoveBuff(info.id);
    }

    // 틱 데미지
    private async UniTask TakeTickDamaged(BaseController target, BuffInfo info, CancellationTokenSource token, int level)
    {
        int tickCount = (int)(info.durationTime / info.tick);
        float tickDamage = GetAmountByLevel(info, level);

        try
        {
            for (int i = 0; i < tickCount; i++)
            {
                if (target == null || !target.buffDic.ContainsKey(info.id))
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

    private void RemoveParticle(Buff buff)
    {
        if (buff.particle != null)
        {
            buff.particle.OnDespawn();
            buff.particle = null;
        }
    }
}
