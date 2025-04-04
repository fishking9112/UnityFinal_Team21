using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
{
    [SerializeField] private GameObject[] prefabs;
    private Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();

    [SerializeField] private int poolSize = 10;

    protected override void Awake()
    {
        base.Awake();

        CreatePool(poolSize);
    }

    public void CreatePool(int size)
    {
        for(int i = 0; i < prefabs.Length; i++)
        {
            if (!pools.ContainsKey(prefabs[i].name))
            {
                ObjectPool newPool = new ObjectPool(prefabs[i].name,prefabs[i], size);
                pools.Add(prefabs[i].name, newPool);
            }
        }

    }

    public GameObject GetObject(string poolName)
    {
        if (!pools.ContainsKey(poolName))
        {
            Debug.Log($"{poolName}(이)라는 풀이 존재하지 않습니다.");
            return null;
        }

        return pools[poolName].GetObject();
    }

    public void ReturnObject(string poolName, GameObject obj)
    {
        if (!pools.ContainsKey(poolName))
        {
            Debug.Log($"{poolName}(이)라는 풀이 존재하지 않습니다.");
            Destroy(obj);
            return;
        }

        pools[poolName].ReturnObject(obj);
    }
}
