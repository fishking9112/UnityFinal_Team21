using System;
using UnityEngine;

public class Return : MonoBehaviour,IPoolable
{
    private Action<GameObject> returnToPool;

    public float checkTime = 3f;
    public float curTime = 0;

    private void Update()
    {
        curTime += Time.deltaTime;

        if (curTime >= checkTime)
        {
            OnDespawn();
        }
    }

    public void Init(Action<GameObject> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnSpawn()
    {
        Debug.Log("스폰");

        curTime = 0;
    }

    public void OnDespawn()
    {
        returnToPool?.Invoke(gameObject);
    }
}