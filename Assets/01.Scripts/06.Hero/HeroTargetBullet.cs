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

    public void Init(Action<Component> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnDespawn()
    {
        isDispose= true;
        cancel?.Cancel();
        cancel?.Dispose();
        returnToPool?.Invoke(this);
    }

    public void OnSpawn()
    {

    }

    public void SetBullet(float dmg,float spd,float knockback)
    {
        damage = dmg;
        speed = spd;
        this.knockback = knockback;
        cancel=new CancellationTokenSource();
        isDispose = false;
        Move(cancel.Token).Forget();
    }


    private async UniTask Move(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        catch
        {
            OnDespawn();
        }
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
            }

        }
    }

}
