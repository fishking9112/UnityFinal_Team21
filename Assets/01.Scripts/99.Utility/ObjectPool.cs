using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private GameObject parentPool;
    private Stack<GameObject> pool;

    public ObjectPool(string poolName, GameObject prefab, int size)
    {
        this.prefab = prefab;
        pool = new Stack<GameObject>();

        parentPool = new GameObject(poolName);
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab, parentPool.transform);
            obj.SetActive(false);
            
            pool.Push(obj);
        }
    }

    public GameObject GetObject()
    {
        GameObject obj;

        if (pool.TryPop(out obj))
        {
            obj.SetActive(true);
        }
        else
        {
            obj = Instantiate(prefab, parentPool.transform);
        }
        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Push(obj);
    }
}
