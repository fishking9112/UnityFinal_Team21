using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class ParticleObject : MonoBehaviour, IPoolable
{
    private Action<Component> returnToPool;
    private ParticleSystem particle;
    private Transform poolParent;

    private void Awake()
    {
        particle = GetComponent<ParticleSystem>();
    }

    public void Init(Action<Component> returnAction)
    {
        returnToPool = returnAction;
        poolParent = transform.parent;
    }

    public void OnSpawn()
    {
        particle.Clear();
        particle.Play();

        // 루프가 아니면 파티클이 끝날 때 자동 반환
        if (!particle.main.loop)
        {
            FinishedReturnToPool().Forget();
        }
    }

    // 파티클이 루프일 경우 수동으로 Despawn 해야 됨
    public void OnDespawn()
    {
        if (poolParent != null)
        {
            transform.SetParent(poolParent);
        }
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        returnToPool?.Invoke(this);
    }

    // 파티클이 끝나면 자동으로 풀에 반환
    private async UniTask FinishedReturnToPool()
    {
        await UniTask.WaitUntil(() => !particle.IsAlive(true));
        OnDespawn();
    }
}
