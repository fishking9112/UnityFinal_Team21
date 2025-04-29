using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Android.Types;
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
        if (target == null)
        {
            return;
        }
        if (!buffDic.TryGetValue(id, out var buffInfo))
        {
            return;
        }

        Utils.Log($"버프 적용 : {buffInfo.name} (ID: {id}, Level: {level})");

        if (target.buffDic.TryGetValue(id, out var existingBuffs))
        {
            Utils.Log($"현재 중첩된 {buffInfo.name} 버프 개수: {existingBuffs.Count}");
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
                    // 현재 적용되어 있는 버프가 지금 적용하려는 버프랑 레벨이 같을 경우 시간만 갱신(새로운 토큰으로 변경)
                    Utils.Log($"{buffInfo.name} : 동일 버프 적용 중 시간 갱신");

                    target.RemoveBuff(id, true);
                    var updateToken = new CancellationTokenSource();
                    target.AddBuff(id, level, updateToken);

                    await ApplyBuffDurationTime(target, buffInfo, updateToken);
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
        else
        {
            // 버프 적용 (이미 버프가 걸려있는 지는 중요하지 않음)
            CancellationTokenSource token = AddBuff(target, buffInfo, level);
            await ApplyBuffDurationTime(target, buffInfo, token);
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
    private CancellationTokenSource AddBuff(BaseController target, BuffInfo info, int level)
    {
        CancellationTokenSource token = new CancellationTokenSource();
        target.AddBuff(info.id, level, token);

        float amount = GetAmountByLevel(info, level);

        switch (info.type)
        {
            case BuffType.ATTACK_DMG:
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
                ApplyParticle(target, info.id, "Burn", target.transform.position, Quaternion.identity, 0.5f, target.transform);
                _ = TakeTickDamaged(target, info, token, level);
                break;
        }

        return token;
    }

    // 버프 제거
    public void RemoveBuff(BaseController target, BuffInfo info)
    {
        if (target.buffDic.TryGetValue(info.id, out var buffList))
        {
            for (int i = buffList.Count - 1; i >= 0; i--)
            {
                var buff = buffList[i];

                if (buff == null)
                {
                    continue;
                }

                target.RemoveBuff(info.id);

                float amount = GetAmountByLevel(info, buff.level);

                switch (info.type)
                {
                    case BuffType.ATTACK_DMG:
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
                        if (buff.particle != null)
                        {
                            buff.particle.OnDespawn();
                            buff.particle = null;
                        }
                        break;
                }

                // 유효 인덱스일 경우에만 제거
                if (i >= 0 && i < buffList.Count)
                {
                    buffList.RemoveAt(i);
                }
            }
        }
    }

    // 틱 데미지
    private async UniTask TakeTickDamaged(BaseController target, BuffInfo info, CancellationTokenSource token, int level)
    {
        int tickCount = (int)(info.durationTime / info.tick);
        float tickDamage = GetAmountByLevel(info, level);

        try
        {
            Utils.Log($"틱 데미지 {tickDamage}씩 {tickCount}번 적용");

            for (int i = 0; i < tickCount; i++)
            {
                if (target == null || !target.buffDic.ContainsKey(info.id))
                {
                    break;
                }
                Utils.Log($"틱 데미지 적용 {tickDamage}");
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

    private void ApplyParticle(BaseController target, int buffId, string key, Vector2 position, Quaternion rotation, float scale, Transform parent)
    {
        if (!target.buffDic.TryGetValue(buffId, out var buffList))
        {
            return;
        }

        foreach (var exisitBuff in buffList)
        {
            if (exisitBuff.particle != null && exisitBuff.particle.gameObject.activeSelf)
            {
                return;
            }
        }

        ParticleObject burnParticle = ParticleManager.Instance.SpawnParticle(key, position, rotation, scale, parent);

        if (buffList.Count > 0)
        {
            buffList[buffList.Count - 1].particle = burnParticle;
        }
    }
}
