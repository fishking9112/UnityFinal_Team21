using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThunderEffect : MonoBehaviour,IPoolable
{
    private Action<Component> returnToPool;

    private float damage;
    private float knockback;
    private float size;
    private float damageRange;
    private ParticleSystem particle;

    public void SetData(float dmg, float kback, float size,float dmgRange)
    {
        damage = dmg;
        knockback = kback;
        this.size = size;
        damageRange= dmgRange*size;

        transform.localScale= Vector3.one*size;

        Overlap().Forget();

    }


    public void Init(Action<Component> returnAction)
    {
        returnToPool = returnAction;
        particle = GetComponentInChildren<ParticleSystem>();


    }

    public void OnDespawn()
    {
        returnToPool?.Invoke(this);
    }

    public void OnSpawn()
    {

    }


    private async UniTask Overlap()
    {
        await UniTask.WaitForSeconds(0.25f,false,PlayerLoopTiming.Update,this.destroyCancellationToken);

        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, damageRange, 1<<7|1<<13);

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
        }
    }

    public bool IsAlive()
    {
        return particle.IsAlive(true);

    }


}
