using System;
using System.Collections.Generic;
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

public class ObjectPoolManager : MonoSingleton<ObjectPoolManager>
{
    [SerializeField] private GameObject[] prefabs;
    private Dictionary<int, Stack<GameObject>> pools = new Dictionary<int, Stack<GameObject>>();
    private Dictionary<int, GameObject> parentPools= new Dictionary<int, GameObject>();

    [SerializeField] private int initPoolSize = 10;

    protected override void Awake()
    {
        base.Awake();

        InitPools();
    }

    // 지정해둔 프리팹 초기 풀 생성
    private void InitPools()
    {
        for (int i = 0; i < prefabs.Length; i++)
        {
            pools[i] = new Stack<GameObject>();

            GameObject parentPool = new GameObject($"Pool_{prefabs[i].name}");
            parentPool.transform.SetParent(this.transform);
            parentPools[i] = parentPool;

            int cloneIndex = i;

            for(int j=0;j< initPoolSize; j++)
            {
                GameObject obj = Instantiate(prefabs[i], parentPool.transform);
                obj.SetActive(false);
                obj.GetComponent<IPoolable>()?.Init(o => ReturnObject(cloneIndex, o));
                pools[i].Push(obj);
            }
        }
    }


    /// <summary>
    /// 풀에서 오브젝트를 가져옴
    /// </summary>
    /// <param name="index"> 가져올 프리팹 인덱스 </param>
    /// <param name="position"> 가져올 position </param>
    /// <param name="rotation"> 가져올 rotation </param>
    /// <returns></returns>
    public GameObject GetObject(int index, Vector3 position, Quaternion rotation)
    {
        if (!pools.ContainsKey(index))
        {
            Debug.Log($"{index}번째 프리팹에 대한 풀이 존재하지 않습니다.");

            return null;
        }

        GameObject obj;

        if (!(pools[index].TryPop(out obj)))
        {
            obj = Instantiate(prefabs[index], parentPools[index].transform);
            obj.GetComponent<IPoolable>()?.Init(o => ReturnObject(index, o));
        }
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        obj.GetComponent<IPoolable>()?.OnSpawn();

        return obj;
    }

    /// <summary>
    /// 풀에 오브젝트를 반환함
    /// </summary>
    /// <param name="index"> 반환할 프리팹 인덱스 </param>
    /// <param name="obj"> 반환할 오브젝트 </param>
    public void ReturnObject(int index, GameObject obj)
    {
        if (!pools.ContainsKey(index))
        {
            Debug.Log($"{index}번째 프리팹에 대한 풀이 존재하지 않습니다.");

            Destroy(obj);
            return;
        }

        obj.SetActive(false);
        pools[index].Push(obj);
    }
}