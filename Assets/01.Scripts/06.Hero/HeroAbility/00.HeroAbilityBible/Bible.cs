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

    private void Update()
    {
        if(target == null)
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
}
