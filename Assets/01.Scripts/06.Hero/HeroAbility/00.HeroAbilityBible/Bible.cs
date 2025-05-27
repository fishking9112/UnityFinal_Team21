using System;
using UnityEditor;
using UnityEngine;

public class Bible : MonoBehaviour, IPoolable
{
    private Action<Component> returnToPool;

    public Transform target;
    public float radius;
    public float speed;
    public float angle;
    public float damage;
    public float knockback;
    private LayerMask targetLayer;

    private void Start()
    {
        targetLayer = LayerMask.GetMask("Monster", "Castle");

    }

    private void Update()
    {
        if (target == null)
        {
            return;
        }

        // target을 중심으로 radius만큼 떨어져서 공전
        angle += speed * Time.deltaTime;
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
        transform.position = target.position + offset;
    }

    public void Init(Action<Component> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnSpawn()
    {

    }

    public void OnDespawn()
    {
        returnToPool?.Invoke(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((1 << collision.gameObject.layer) & targetLayer) != 0)
        {
            if (MonsterManager.Instance.monsters.TryGetValue(collision.gameObject, out var monster))
            {
                monster.TakeKnockback(target.transform, knockback);
                monster.TakeDamaged(damage);
            }
            else if (GameManager.Instance.castle.gameObject == collision.gameObject)
            {
                GameManager.Instance.castle.TakeDamaged(damage);
            }
            else if (GameManager.Instance.miniCastles.TryGetValue(collision.gameObject, out var miniCastle))
            {
                miniCastle.TakeDamaged(damage);
            }
        }

    }

}
