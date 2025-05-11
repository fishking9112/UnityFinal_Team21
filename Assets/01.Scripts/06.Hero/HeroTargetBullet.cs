using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HeroTargetBullet : MonoBehaviour,IPoolable
{
    private GameObject target;

    private float speed;
    private float damage;

    float knockback;
    CancellationTokenSource cancel;
    private bool isDispose;
    private Action<Component> returnToPool;

    private Vector3 targetPos;

    public void Init(Action<Component> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnDespawn()
    {
        isDispose= true;

        try
        {

            cancel?.Cancel();
            cancel?.Dispose();
        }
        catch
        {

        }
        returnToPool?.Invoke(this);
    }

    public void OnSpawn()
    {

    }

    public void SetBullet(float dmg,float spd,float knockback,GameObject t)
    {
        damage = dmg;
        speed = spd;
        this.knockback = knockback;
        cancel=new CancellationTokenSource();
        isDispose = false;

        targetPos = t.transform.position;
        
        Move(cancel.Token).Forget();
    }


    private async UniTask Move(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                Vector3 pos= Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                transform.position = pos;

                if(Vector3.Distance(transform.position,targetPos)<=0.1f)
                {
                    Explode();
                    return;
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        catch
        {

        }
        finally
        {
            OnDespawn();
        }
    }

    private void Explode()
    {
        Collider2D[] col = Physics2D.OverlapCircleAll(transform.position, 3, 1 << 7| 1<<13);

        foreach (var c in col)
        {
            if (MonsterManager.Instance.monsters.TryGetValue(c.gameObject, out var monster))
            {
                monster.TakeKnockback(this.transform, knockback);
                monster.TakeDamaged(damage);

            }
        }
        OnDespawn();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(isDispose) return;

        if(collision.gameObject==target)
        {
            if (MonsterManager.Instance.monsters.TryGetValue(collision.gameObject, out var monster))
            {
                monster.TakeKnockback(this.transform, knockback);
                monster.TakeDamaged(damage);

                OnDespawn();
            }

        }
    }

}
