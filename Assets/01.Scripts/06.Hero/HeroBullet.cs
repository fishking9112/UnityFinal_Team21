using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class HeroBullet : MonoBehaviour
{
    private Vector2 dir;
    private float speed;
    private float damage;
    private LayerMask targetLayer;

    float time;
    float limitTime;

    /// <summary>
    /// 총알 세팅(발사할때 하면 되나?)
    /// </summary>
    /// <param name="direction">이동방향</param>
    /// <param name="moveSpeed">이동속도</param>
    /// <param name="dmg">대미지</param>
    /// <param name="layer">충돌할 레이어</param>
    public void Init(Vector2 direction, float moveSpeed, float dmg,int layer)
    {
        dir = direction;
        speed = moveSpeed;
        damage = dmg;
        time = 0f;
        targetLayer = 1 << layer;
        limitTime = 5f;
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
            transform.position = (Vector2)transform.position + speed * Time.deltaTime * dir;
            time += Time.deltaTime;

            await UniTask.Yield();
        }

        // 오브젝트풀.릴리즈
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 딕셔너리 통해서 GetComponent 없이 쓰기
    }

}
