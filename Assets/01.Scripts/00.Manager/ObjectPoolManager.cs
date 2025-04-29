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

        private Action<Component> returnToPool;

        public void Init(Action<Component> returnAction)
        {
            returnToPool = returnAction;
        }

        public void OnDespawn()
        {
            returnToPool?.Invoke(this);
        }
    */

    void Init(Action<Component> returnAction);
    void OnSpawn();
    void OnDespawn();
}

[Serializable]
public class PrefabType
{
    public string key;
    public Component prefab;
    public int initPoolSize;

    public PrefabType(Component prefab)
    {
        this.prefab = prefab;
    }
}
public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
{
    [SerializeField] private PrefabType[] poolPrefabs;

    private Dictionary<string, Stack<Component>> pools = new Dictionary<string, Stack<Component>>();
    private Dictionary<string, GameObject> parentMap = new Dictionary<string, GameObject>();
    private Dictionary<string, PrefabType> prefabMap = new Dictionary<string, PrefabType>();

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
            pools[prefab.key] = new Stack<Component>();

            GameObject parentPool = new GameObject($"Pool_{prefab.key}");
            parentPool.transform.SetParent(this.transform);
            parentMap[prefab.key] = parentPool;
            prefabMap[prefab.key] = new PrefabType(prefab.prefab);

            for (int i = 0; i < prefab.initPoolSize; i++)
            {
                Component comp = CreatePool<Component>(prefab.key);
                comp.gameObject.SetActive(false);
                pools[prefab.key].Push(comp);
            }
        }
    }

    private T CreatePool<T>(string key) where T : Component
    {
        var p = prefabMap[key];
        T comp = Instantiate(p.prefab, parentMap[key].transform) as T;

        comp.GetComponent<IPoolable>()?.Init(o => ReturnObject(key, o));
        return comp;
    }

    /// <summary>
    /// 풀에서 오브젝트를 가져옴
    /// </summary>
    /// <param name="index"> 가져올 프리팹 인덱스 </param>
    /// <param name="position"> 가져올 position </param>
    /// <param name="rotation"> 가져올 rotation </param>
    /// <returns></returns>
    public T GetObject<T>(string key, Vector2 position) where T:Component
    {
        if (!pools.ContainsKey(key))
        {
            return null;
        }

        Component comp;

        if (!(pools[key].TryPop(out comp)))
        {
            comp = CreatePool<T>(key);
        }
        comp.transform.position = position;
        comp.gameObject.SetActive(true);
        comp.GetComponent<IPoolable>()?.OnSpawn();
        return comp as T;
    }

    /// <summary>
    /// 풀에 오브젝트를 반환함
    /// </summary>
    private void ReturnObject(string key, Component comp)
    {
        if (!pools.ContainsKey(key))
        {
            Destroy(comp);
            return;
        }

        comp.gameObject.SetActive(false);
        pools[key].Push(comp);
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