using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BuffController : MonoBehaviour
{
    public Dictionary<int, List<Buff>> buffDic = new Dictionary<int, List<Buff>>();

    // 버프 추가
    public Buff AddBuff(int id, int level, CancellationTokenSource token)
    {
        var buff = new Buff(id, level, token);

        if (!buffDic.ContainsKey(id))
        {
            buffDic[id] = new List<Buff>();
        }

        buffDic[id].Add(buff);
        return buff;
    }

    // 버프 제거
    public void RemoveBuff(int id)
    {
        if (!buffDic.TryGetValue(id, out var buffList))
        {
            return;
        }

        foreach (var buff in buffList)
        {
            buff.token?.Cancel();
            buff.token?.Dispose();
        }

        buffList.Clear();
        buffDic.Remove(id);
    }

    // 모든 버프 제거
    public void ClearAllBuff()
    {
        foreach (var key in new List<int>(buffDic.Keys))
        {
            if (buffDic.TryGetValue(key, out var buffList))
            {
                foreach (var buff in buffList)
                {
                    if (buff != null && buff.particle != null)
                    {
                        buff.particle.OnDespawn();
                        buff.particle = null;
                    }
                }
            }
            RemoveBuff(key);
        }
        buffDic.Clear();
    }
}