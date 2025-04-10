using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 오브젝트 풀을 사용할 클래스에 상속받아서 사용
/// </summary>
public interface IPoolable
{
    /*
        IPoolable을 상속받아서 아래와 같이 만들어야 OnDespawn() 호출 시 객체에 맞는 풀로 반환 됨.

        -------------------------------------------------------------------------------------

        private Action<GameObject> returnToPool;

        public void Init(Action<GameObject> returnAction)
        {
            returnToPool = returnAction;
        }

        public void OnDespawn()
        {
            returnToPool?.Invoke(gameObject);
        }
    */

    void Init(Action<GameObject> returnAction);
    void OnSpawn();
    void OnDespawn();
}

[Serializable]
public class PoolPrefab
{
    public string key;
    public GameObject prefab;
    public int initPoolSize;
}

public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
{
    [SerializeField] private PoolPrefab[] poolPrefabs;

    private Dictionary<string, Stack<GameObject>> pools = new Dictionary<string, Stack<GameObject>>();
    private Dictionary<string, GameObject> parentMap = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> prefabMap = new Dictionary<string, GameObject>();

    protected override void Awake()
    {
        base.Awake();

        InitPools();
    }

    // 지정해둔 프리팹 초기 풀 생성
    private void InitPools()
    {
        foreach (var prefab in poolPrefabs)
        {
            pools[prefab.key] = new Stack<GameObject>();

            GameObject parentPool = new GameObject($"Pool_{prefab.key}");
            parentPool.transform.SetParent(this.transform);
            parentMap[prefab.key] = parentPool;
            prefabMap[prefab.key] = prefab.prefab;

            for (int i = 0; i < prefab.initPoolSize; i++)
            {
                GameObject obj = CreatePool(prefab.key);
                obj.SetActive(false);
                pools[prefab.key].Push(obj);
            }
        }
    }

    private GameObject CreatePool(string key)
    {
        GameObject obj = Instantiate(prefabMap[key], parentMap[key].transform);
        obj.GetComponent<IPoolable>()?.Init(o => ReturnObject(key, o));
        return obj;
    }

    /// <summary>
    /// 풀에서 오브젝트를 가져옴
    /// </summary>
    /// <param name="index"> 가져올 프리팹 인덱스 </param>
    /// <param name="position"> 가져올 position </param>
    /// <param name="rotation"> 가져올 rotation </param>
    /// <returns></returns>
    public GameObject GetObject(string key, Vector2 position)
    {
        if (!pools.ContainsKey(key))
        {
            return null;
        }

        GameObject obj;

        if (!(pools[key].TryPop(out obj)))
        {
            obj = CreatePool(key);
        }
        obj.transform.position = position;
        obj.SetActive(true);
        obj.GetComponent<IPoolable>()?.OnSpawn();

        return obj;
    }

    /// <summary>
    /// 풀에 오브젝트를 반환함
    /// </summary>
    private void ReturnObject(string key, GameObject obj)
    {
        if (!pools.ContainsKey(key))
        {
            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        pools[key].Push(obj);
    }

    // key값에 대한 풀 초기화
    public void ClearPool(string key)
    {
        if (!pools.ContainsKey(key))
        {
            return;
        }

        while (pools[key].Count > 0)
        {
            Destroy(pools[key].Pop());
        }

        pools[key].Clear();
    }

    // 모든 풀 초기화
    public void ClearAllPools()
    {
        foreach (var key in pools.Keys)
        {
            ClearPool(key);
        }
    }
}