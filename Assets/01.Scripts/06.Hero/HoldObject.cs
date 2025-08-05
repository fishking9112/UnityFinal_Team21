using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class HoldObject : MonoBehaviour, IPoolable
{

    private Action<Component> returnToPool;

    private float duration;
    private float damage;
    private float knockback;
    private float size;
    private float damageRange;
    private float damageDelay;

    private CancellationTokenSource token;

    public void SetData(float dur, float dmg, float kback, float size, float dmgRange, float dmgDelay)
    {
        duration = dur;
        damage = dmg;
        knockback = kback;
        this.size = size;
        damageRange = dmgRange * size;
        damageDelay = dmgDelay;
        transform.localScale = Vector3.one * size;
        token = new CancellationTokenSource();
        Overlap(token.Token).Forget();

    }


    public void Init(Action<Component> returnAction)
    {
        returnToPool = returnAction;


    }

    public void OnDespawn()
    {
        returnToPool?.Invoke(this);
        token?.Cancel();
        token?.Dispose();
    }

    public void OnSpawn()
    {
        LifeCycle().Forget();
    }
    private async UniTask LifeCycle()
    {
        await UniTask.WaitForSeconds(duration, false, PlayerLoopTiming.Update, this.destroyCancellationToken);
        OnDespawn();
    }


    private async UniTask Overlap(CancellationToken tk)
    {
        while (!tk.IsCancellationRequested)
        {
            await UniTask.WaitForSeconds(damageDelay, false, PlayerLoopTiming.Update, this.destroyCancellationToken);

            Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, damageRange, 1 << 7 | 1 << 13);

            foreach (Collider2D c in col)
            {
                if (MonsterManager.Instance.monsters.TryGetValue(c.gameObject, out var monster))
                {
                    monster.TakeKnockback(transform, knockback);
                    monster.TakeDamaged(damage);
                }
                else if (GameManager.Instance.castle.gameObject == c.gameObject)
                {
                    GameManager.Instance.castle.TakeDamaged(damage);
                }
                else if (GameManager.Instance.miniCastles.TryGetValue(c.gameObject, out var miniCastle))
                {
                    miniCastle.TakeDamaged(damage);
                }
            }
        }
    }


}
