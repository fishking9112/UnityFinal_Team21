using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Pooling/PoolSetting")]
public class Pooling : ScriptableObject
{
    public List<PoolObject> poolList;
}
[System.Serializable]
public class PoolObject
{
    public string key;
    public GameObject prefab;
    public int initSize;
}
