using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class HeroBullet : MonoBehaviour , IPoolable
{
    private Vector2 dir;
    private float speed;
    private float damage;
    private LayerMask targetLayer;

    Rigidbody2D rigid;
    float time;
    float limitTime;
    float pierce;
    float rotateSpeed;
    private Action<Component> returnToPool;
    CancellationTokenSource cancel = new CancellationTokenSource();

    CancellationToken token;

    public void SetBullet(float time,float pierceCnt,float dmg,float spd,float rSpeed)
    {
        limitTime = time;
        pierce = pierceCnt;
        damage = dmg;
        speed = spd;
        rotateSpeed = rSpeed;
        Move().Forget();
    }

    public void Init(Action<Component> returnAction)
    {
        returnToPool = returnAction;
        targetLayer = 7;
        token = this.GetCancellationTokenOnDestroy();
    }

    public void OnDespawn()
    {
        time = 0f;

        cancel.Cancel();
        returnToPool?.Invoke(this);
    }

    public void OnSpawn()
    {
        
    }


    /// <summary>
    /// 5초간 이동하다가 오브젝트풀 릴리즈 하기
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid Move()
    {
        
        while (time < limitTime)
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
        // 딕셔너리 통해서 GetComponent 없이 쓰기
        if (collision.gameObject.layer == targetLayer)
        {
            if (MonsterManager.Instance.monsters.TryGetValue(collision.gameObject, out var monster))
            {
                monster.TakeDamaged(damage);
            }
            pierce--;
            if (pierce <= 0)
            {
                OnDespawn();
            }
        }
    }

}
