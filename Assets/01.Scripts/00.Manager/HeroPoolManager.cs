using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

    // UI 관련
    [Header("Name Tag Settings")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private Canvas worldCanvas;
    [SerializeField] private TextMeshProUGUI nameTagPrefab;

    private readonly Queue<TextMeshProUGUI> nameTagPool = new Queue<TextMeshProUGUI>();
    private readonly List<(TextMeshProUGUI ui, Transform target)> activeNameTags = new List<(TextMeshProUGUI, Transform)>();

    private List<GameObject> list;
    private List<GameObject> bossList;

    private List<HeroController> poolList = new List<HeroController>();
    private List<GameObject> heroList = new List<GameObject>();

    private Dictionary<HeroController, GameObject> heroDic = new Dictionary<HeroController, GameObject>();

    private QueenCondition condition;

    private Transform heroParent;
    private Transform heroDeleteParent;

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

        GameObject heroP = new GameObject();
        heroParent = heroP.transform;
        heroParent.SetParent(transform);

        GameObject heroDP = new GameObject();
        heroDeleteParent = heroDP.transform;
        heroDeleteParent.SetParent(transform);

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

    void Start()
    {
        if (heroObj == null)
        {
            return;
        }
        condition = GameManager.Instance.queen.condition;
    }

    protected override void OnDestroy()
    {
        AddressableManager.Instance.ReleaseAsset("Hero");
        AddressableManager.Instance.ReleaseAsset("BossHero");

        base.OnDestroy();
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

                obj.transform.position = pos;
                obj.gameObject.SetActive(true);
                HeroManager.Instance.hero[obj.gameObject] = obj;
                heroDic[obj] = hPrefab;

                // 10% 확률로 이름/레벨 표기
                if (UnityEngine.Random.value <= 0.1f && DataManager.Instance.heroNameDic.Count > 0)
                {
                    var keys = new List<int>(DataManager.Instance.heroNameDic.Keys);
                    int randomKey = keys[UnityEngine.Random.Range(0, keys.Count)];
                    string heroName = DataManager.Instance.heroNameDic[randomKey].Name;

                    int heroLevel = HeroManager.Instance.Level;
                    string displayText = $"Lv.[ {heroLevel} ] {heroName}";

                    ShowNameTag(obj.transform, displayText);
                }

                return obj;
            }
        }
        return null;
    }

    private void ShowNameTag(Transform target, string text)
    {
        TextMeshProUGUI ui = nameTagPool.Count > 0 ? nameTagPool.Dequeue() : Instantiate(nameTagPrefab, worldCanvas.transform);
        ui.text = text;
        ui.gameObject.SetActive(true);
        activeNameTags.Add((ui, target));
    }

    void LateUpdate()
    {
        for (int i = activeNameTags.Count - 1; i >= 0; i--)
        {
            var (ui, target) = activeNameTags[i];
            if (target == null || !target.gameObject.activeInHierarchy)
            {
                ui.gameObject.SetActive(false);
                nameTagPool.Enqueue(ui);
                activeNameTags.RemoveAt(i);
                continue;
            }

            Vector3 worldPos = target.position + new Vector3(0, 2f, 0);
            ui.transform.position = worldPos;
        }
    }

    public void ReturnObject(HeroController obj)
    {
        if (obj == null || obj.gameObject == null) return;

        HeroManager.Instance.hero.Remove(obj.gameObject);

        // 보스일경우
        if (!heroDic.ContainsKey(obj))
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
            heroDic[obj].transform.SetParent(heroDeleteParent);
            Destroy(heroDic[obj]);
            heroDic.Remove(obj);
        }

        if (!GameManager.Instance.gameResultController.gameEnd)
        {
            condition.KillCnt.Value++;
        }

        obj.gameObject.SetActive(false);
    }
}
