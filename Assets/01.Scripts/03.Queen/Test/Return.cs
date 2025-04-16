using System;
using System.Collections;
using UnityEngine;

public class Return : MonoBehaviour, IPoolable
{
    private Action<Component> returnToPool;
    private IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(2.0f);
        OnDespawn();
    }

    public void Init(Action<Component> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnSpawn()
    {
        StartCoroutine(ReturnToPool());
        //GameManger.Instacnce.MonsterMap.Add(this.gameObject,this);
    }

    public void OnDespawn()
    {
        //GameManager.Instance.MosterMap.Remove(this.gameObject,this);
    }
}
