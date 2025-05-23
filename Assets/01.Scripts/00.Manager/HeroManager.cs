using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class HeroManager : MonoSingleton<HeroManager>
{
    public Dictionary<GameObject, HeroController> hero = new Dictionary<GameObject, HeroController>();

    public Dictionary<int, HeroAbilitySystem> allAbilityDic = new Dictionary<int, HeroAbilitySystem>();

    private HeroStatusInfo statusInfo;

    public bool isHealthUI = false;
    private CancellationTokenSource token;
    private CancellationTokenSource token2;

    private float time;
    private int heroCnt;

    private List<GameObject> heroList = new();

    private void Start()
    {
        time = 10;
        heroCnt = 1;
        token = new CancellationTokenSource();
        token2 = new CancellationTokenSource();
        SetWave(token.Token).Forget();
        IncreaseCnt(token2.Token).Forget();
    }

    private async UniTask IncreaseCnt(CancellationToken tk)
    {
        while (!tk.IsCancellationRequested)
        {
            await UniTask.Delay(TimeSpan.FromMinutes(1), cancellationToken: this.GetCancellationTokenOnDestroy());
            heroCnt++;
        }
    }

    private async UniTask SetWave(CancellationToken tk)
    {
        while (!tk.IsCancellationRequested) // 게임 끝나기 전까지
        {
            await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: this.GetCancellationTokenOnDestroy()); //매 10초마다


            for (int i = 0; i < heroCnt; i++)
            {
                SetNextHero();
                SummonHero();
            }
        }
    }

    private void SetNextHero()
    {
        var list = DataManager.Instance.heroStatusDic.Where(x => x.Value.heroType == HeroType.NORMAL).Select(x=>x.Value).ToList();

        var val = list[UnityEngine.Random.Range(0, list.Count)];

        statusInfo = val;

    }


    private void SummonHero()
    {
        if (HeroPoolManager.Instance == null)
        {
            return;
        }

        HeroController hero = HeroPoolManager.Instance.GetObject(SpawnPointManager.Instance.heroPoint.GetRandomPosition());
        hero?.StatInit(statusInfo, HeroManager.Instance.isHealthUI);
    }

    public List<GameObject> SummonHeros(Vector2 v, int count, bool isEventMark = true)
    {
        heroList.Clear();

        for (int i = 0; i < count; i++)
        {
            SetNextHero();
            HeroController hero = HeroPoolManager.Instance.GetObject(GetRandomPos(v, 3));
            hero?.StatInit(statusInfo, HeroManager.Instance.isHealthUI, isEventMark);
            heroList.Add(hero.gameObject);
        }
        return heroList;
    }

    public GameObject SummonBoss(Vector2 v, int type, bool isEventMark = true)
    {
        //int cnt = DataManager.Instance.heroStatusDic.Select(x=>x.Value.id==201);

        statusInfo = DataManager.Instance.heroStatusDic[type];

        // HeroController boss = HeroPoolManager.Instance.GetBossObject(RandomSummonPos(90, 90));
        HeroController boss = HeroPoolManager.Instance.GetBossObject(GetRandomPos(v, 3));
        boss?.StatInit(statusInfo, HeroManager.Instance.isHealthUI, isEventMark);

        return boss.gameObject;

    }

    private Vector2 GetRandomPos(Vector2 pos, float radius = 3f)
    {
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2);
        float distance = UnityEngine.Random.Range(0f, radius);
        float x = pos.x + Mathf.Cos(angle) * distance;
        float y = pos.y + Mathf.Sin(angle) * distance;
        return new Vector2(x, y);

    }

    private Vector2 RandomSummonPos(float width, float height, float edge = 5f)
    {
        int edgeType = UnityEngine.Random.Range(0, 4);
        float halfW = width * 0.5f;
        float halfH = height * 0.5f;

        float x = 0f, z = 0f;

        if (edgeType < 2)
        {
            x = UnityEngine.Random.Range(-halfW, halfW);
            z = (edgeType == 0)
                ? UnityEngine.Random.Range(halfH - edge, halfH)
                : UnityEngine.Random.Range(-halfH, -halfH + edge);
        }
        else
        {
            z = UnityEngine.Random.Range(-halfH, halfH);
            x = (edgeType == 2)
                ? UnityEngine.Random.Range(-halfW, -halfW + edge)
                : UnityEngine.Random.Range(halfW - edge, halfW);
        }

        return new Vector2(x, z);

    }
    public void OnClickHealthUITest()
    {
        isHealthUI = !isHealthUI;
        foreach (var _hero in hero)
        {
            _hero.Value.SetHealthUI(isHealthUI);
        }
    }
}
