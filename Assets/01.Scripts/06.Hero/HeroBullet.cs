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
    private Action<GameObject> returnToPool;

    CancellationToken token;
    private void OnEnable()
    {
        speed = 3f;
        damage = 1;
    }

    public void Init(Action<GameObject> returnAction)
    {
        returnToPool = returnAction;
        limitTime = 5f;
        token = this.GetCancellationTokenOnDestroy();
    }

    public void OnDespawn()
    {
        time = 0f;

        returnToPool?.Invoke(gameObject);
    }

    public void OnSpawn()
    {
        Move().Forget();
    }


    /// <summary>
    /// 5초간 이동하다가 오브젝트풀 릴리즈 하기
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid Move()
    {
        while (time < limitTime)
        {
            transform.position = (Vector2)transform.position + speed * Time.deltaTime * (Vector2)transform.up;
            time += Time.deltaTime;

            await UniTask.Yield(cancellationToken:token);
        }

        OnDespawn();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 딕셔너리 통해서 GetComponent 없이 쓰기
    }

}
