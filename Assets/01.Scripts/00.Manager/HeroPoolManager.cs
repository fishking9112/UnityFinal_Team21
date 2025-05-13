using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class HeroObj
{
    public HeroController obj;
    public int poolSize;
}

public class HeroPoolManager : MonoSingleton<HeroPoolManager>
{
    [SerializeField] private HeroObj heroObj;
    [SerializeField] private HeroController bossObj;

    private List<GameObject> list;
    private List<GameObject> bossList;

    private List<HeroController> poolList = new List<HeroController>();

    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        if(heroObj == null)
        {
            return;
        }

        list = AddressableManager.Instance.LoadDataAssets<GameObject>("Hero");
        bossList = AddressableManager.Instance.LoadDataAssets<GameObject>("BossHero");
        System.Random rand = new System.Random();
        list = list.OrderBy(x => rand.Next()).ToList();
        int min = Mathf.Min(list.Count, heroObj.poolSize);

        for(int i=0; i<min; i++)
        {
            HeroController hObj = Instantiate(heroObj.obj, transform);
            GameObject hPrefab = Instantiate(list.ElementAt(i), Vector3.zero, Quaternion.identity, hObj.transform);
            hObj.InitHero();
            hObj.gameObject.SetActive(false);
            poolList.Add(hObj);
        }
    }

    public HeroController GetBossObject(Vector2 pos)
    {
        int rand=UnityEngine.Random.Range(0,bossList.Count);
        HeroController hObj = Instantiate(bossObj, transform);
        GameObject hPrefab = Instantiate(bossList.ElementAt(rand), Vector3.zero, Quaternion.identity, hObj.transform);
        hObj.InitHero();
        hObj.transform.position = pos;

        return hObj;
    }

    public HeroController GetObject(Vector2 pos)
    {
        foreach (var obj in poolList)
        {
            if (!obj.gameObject.activeSelf)
            {
                // 세팅은 받은 쪽에서 하기
                obj.transform.position = pos;
                obj.gameObject.SetActive(true);
                HeroManager.Instance.hero[obj.gameObject] = obj;
                return obj;
            }
        }

        // 남은게없을경우(최대치 다 나갔을 경우)
        return null;
    }

    public void ReturnObject(HeroController obj)
    {
        HeroManager.Instance.hero.Remove(obj.gameObject);
        obj.gameObject.SetActive(false);
    }
}
