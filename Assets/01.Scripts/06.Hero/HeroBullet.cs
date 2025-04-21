using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime;
using System.ComponentModel.Design;

public class HeroBullet : MonoBehaviour , IPoolable
{
    
    private Vector2 dir;
    private float speed;
    private float damage;
    private LayerMask targetLayer;
    private LayerMask obstacleLayer;

    Rigidbody2D rigid;
    float time;
    float limitTime;
    float pierce;
    float rotateSpeed;
    private Action<Component> returnToPool;
    CancellationTokenSource cancel = new CancellationTokenSource();
    private bool isDispose;


    public void SetBullet(float time,float pierceCnt,float dmg,float spd,float rSpeed)
    {
        limitTime = time;
        pierce = pierceCnt;
        damage = dmg;
        speed = spd;
        rotateSpeed = rSpeed;
        isDispose = false;
        Move(cancel.Token).Forget();
    }

    public void Init(Action<Component> returnAction)
    {
        returnToPool = returnAction;
        targetLayer = 7;
        obstacleLayer = 10;
    }

    public void OnDespawn()
    {
        time = 0f;
        isDispose = true;
        cancel?.Cancel();
        cancel?.Dispose();
        returnToPool?.Invoke(this);
    }

    public void OnSpawn()
    {
        isDispose = false;
        cancel = new CancellationTokenSource();

    }


    /// <summary>
    /// 5초간 이동하다가 오브젝트풀 릴리즈 하기
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid Move(CancellationToken token)
    {
        
        while (time < limitTime && !token.IsCancellationRequested)
        {
            transform.Rotate(0,0, rotateSpeed * Time.deltaTime);
            transform.position = (Vector2)transform.position + speed * Time.deltaTime * (Vector2)transform.up;
            time += Time.deltaTime;

            await UniTask.Yield(cancellationToken:token);
        }

        OnDespawn();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDispose)
            return;

        if (collision.gameObject.layer == targetLayer)
        {
            if (MonsterManager.Instance.monsters.TryGetValue(collision.gameObject, out var monster))
            {
                monster.TakeDamaged(damage);

                pierce--;
                if (pierce <= 0)
                {
                    OnDespawn();
                }
            }
        }
        else if(collision.gameObject.layer == obstacleLayer)
        {
            OnDespawn();
        }
    }

}
