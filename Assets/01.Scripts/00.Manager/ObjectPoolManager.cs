using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

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

public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
{
    private Dictionary<string, Stack<Component>> pools = new();
    private Dictionary<string, GameObject> parentMap = new();
    private Dictionary<string, PrefabType> prefabMap = new();

    protected override async void Awake()
    {
        base.Awake();
        await InitPoolsFromAddressables();
    }

    public async UniTask InitPoolsFromAddressables()
    {
        var handle = Addressables.LoadAssetsAsync<GameObject>("poolObj", null);
        var loadedSettings = await handle.ToUniTask();

        foreach (var setting in loadedSettings)
        {

            var p = setting.GetComponent<IPoolable>();

       
            RegisterPool(setting.name, p as Component, 5);
        }

    }

    public void RegisterPool<T>(string key, T prefab, int initPoolSize = 0) where T : Component
    {
        if (pools.ContainsKey(key))
        {
            Utils.LogWarning($"이미 등록되어 있음");
            return;
        }

        pools[key] = new Stack<Component>();

        GameObject parentPool = new GameObject($"Pool_{key}");
        parentPool.transform.SetParent(this.transform);
        prefabMap[key] = new PrefabType(key, prefab, initPoolSize);
        parentMap[key] = parentPool;

        for (int i = 0; i < initPoolSize; i++)
        {
            T comp = CreatePool<T>(key);
            comp.gameObject.SetActive(false);
            pools[key].Push(comp);
        }
    }

    private T CreatePool<T>(string key) where T : Component
    {
        var p = prefabMap[key];
        T comp= Instantiate(p.prefab, parentMap[key].transform) as T;
        comp.GetComponent<IPoolable>()?.Init(o => ReturnObject(key, o));
        return comp;
    }

    public T GetObject<T>(string key, Vector2 position) where T : Component
    {
        if (!pools.ContainsKey(key))
        {
            Utils.LogError($"해당 오브젝트 존재하지 않음");
            return null;
        }

        Component comp;
        if (!pools[key].TryPop(out comp))
        {
            comp = CreatePool<T>(key);
        }

        comp.transform.position = position;
        comp.gameObject.SetActive(true);
        comp.GetComponent<IPoolable>()?.OnSpawn();
        return comp as T;
    }

    private void ReturnObject(string key, Component comp)
    {
        if (comp == null) return;

        if (!pools.ContainsKey(key))
        {
            Destroy(comp);
            return;
        }

        comp.gameObject.SetActive(false);
        pools[key].Push(comp);
    }
}

[Serializable]
public class PrefabType
{
    public string key;
    public Component prefab;
    public int initPoolSize;

    public PrefabType(string key, Component prefab, int initPoolSize)
    {
        this.key = key;
        this.prefab = prefab;
        this.initPoolSize = initPoolSize;
    }
}