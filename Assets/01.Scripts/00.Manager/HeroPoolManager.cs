using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class HeroObj
{
    public GameObject obj;
    public int poolSize;
}

public class HeroPoolManager : MonoSingleton<HeroPoolManager>
{
    [SerializeField] private HeroObj heroObj;

    private List<GameObject> list;

    private List<GameObject> poolList = new List<GameObject>();

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        list = Resources.LoadAll<GameObject>("HeroPrefabs").ToList();
        System.Random rand = new System.Random();
        list = list.OrderBy(x => rand.Next()).ToList();
        int min = Mathf.Min(list.Count, heroObj.poolSize);

        for(int i=0; i<min; i++)
        {
            GameObject hObj = Instantiate(heroObj.obj, transform);
            GameObject hPrefab = Instantiate(list.ElementAt(i), Vector3.zero, Quaternion.identity, hObj.transform);
            hObj.GetComponent<HeroController>().InitHero();
            hObj.SetActive(false);
            poolList.Add(hObj);
        }
    }

    public GameObject GetObject(Vector2 pos)
    {
        foreach (var obj in poolList)
        {
            if (!obj.activeSelf)
            {
                // 세팅은 받은 쪽에서 하기
                obj.SetActive(true);
                return obj;
            }
        }


        // 남은게없을경우(최대치 다 나갔을 경우)
        return null;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }



}
