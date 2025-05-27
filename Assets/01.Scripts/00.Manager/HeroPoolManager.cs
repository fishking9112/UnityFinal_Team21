using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    private List<GameObject> heroList = new List<GameObject>();

    private Dictionary<HeroController, GameObject> heroDic = new Dictionary<HeroController, GameObject>();

    private QueenCondition condition;

    private Transform heroParent;

    protected override void Awake()
    {
        base.Awake();
        InitAsync().Forget();
    }

    private async UniTask InitAsync()
    {
        list = await AddressableManager.Instance.LoadDataAssetsAsync<GameObject>("Hero");
        bossList = await AddressableManager.Instance.LoadDataAssetsAsync<GameObject>("BossHero");
        System.Random rand = new System.Random();
        list = list.OrderBy(x => rand.Next()).ToList();
        int min = Mathf.Min(list.Count, heroObj.poolSize);

        GameObject heroP = new GameObject();
        heroParent = heroP.transform;
        heroParent.SetParent(transform);

        for (int i = 0; i < heroObj.poolSize; i++)
        {
            HeroController obj = Instantiate(heroObj.obj, transform);
            obj.InitHero();
            obj.gameObject.SetActive(false);
            poolList.Add(obj);
        }

        for (int i = 0; i < list.Count; i++)
        {
            GameObject obj = Instantiate(list[i], Vector3.zero, Quaternion.identity, heroParent);
            obj.SetActive(false);
            heroList.Add(obj);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        if (heroObj == null)
        {
            return;
        }
        condition = GameManager.Instance.queen.condition;
    }

    public HeroController GetBossObject(Vector2 pos)
    {
        int rand = UnityEngine.Random.Range(0, bossList.Count);
        HeroController hObj = Instantiate(bossObj, transform);
        GameObject hPrefab = Instantiate(bossList.ElementAt(rand), Vector3.zero, Quaternion.identity, hObj.transform);
        hObj.InitHero();
        hObj.transform.position = pos;
        HeroManager.Instance.hero[hObj.gameObject] = hObj;

        return hObj;
    }

    public HeroController GetObject(Vector2 pos)
    {
        if (poolList.Count == HeroManager.Instance.hero.Count)
        {
            HeroController hObj = Instantiate(heroObj.obj, transform);
            hObj.InitHero();
            poolList.Add(hObj);
            hObj.gameObject.SetActive(false);
        }


        foreach (var obj in poolList)
        {
            if (!obj.gameObject.activeSelf)
            {
                int rand = UnityEngine.Random.Range(0, heroList.Count);
                GameObject hPrefab = heroList.ElementAt(rand);

                if (hPrefab.activeSelf)
                {
                    hPrefab = Instantiate(hPrefab, Vector3.zero, Quaternion.identity, obj.transform);
                    hPrefab.transform.localPosition = Vector3.zero;
                    hPrefab.transform.localScale = Vector3.one;
                }
                else
                {
                    hPrefab.SetActive(true);
                    hPrefab.transform.SetParent(obj.transform);
                    hPrefab.transform.localPosition = Vector3.zero;
                    hPrefab.transform.localScale = Vector3.one;
                }

                // 세팅은 받은 쪽에서 하기
                obj.transform.position = pos;
                obj.gameObject.SetActive(true);
                HeroManager.Instance.hero[obj.gameObject] = obj;
                heroDic[obj] = hPrefab;
                return obj;
            }
        }

        return null;
    }

    public void ReturnObject(HeroController obj)
    {
        HeroManager.Instance.hero.Remove(obj.gameObject);
        obj.gameObject.SetActive(false);

        // 보스일경우
        if(!heroDic.ContainsKey(obj))
        {
            Destroy(obj.gameObject);
        }
        // 보스가 아닌 히어로일경우
        else if (heroList.Contains(heroDic[obj]))
        {
            heroDic[obj].transform.SetParent(heroParent);
            heroDic[obj].SetActive(false);
            heroDic.Remove(obj);
        }
        // 풀링으로 생성된 히어로일경우
        else
        {
            Destroy(heroDic[obj]);
            heroDic.Remove(obj);
        }

        condition.KillCnt.Value++;
    }
}
