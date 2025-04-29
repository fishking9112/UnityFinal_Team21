using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeroManager : MonoSingleton<HeroManager>
{
    public Dictionary<GameObject, HeroController> hero = new Dictionary<GameObject, HeroController>();

    private List<int> idList = new List<int>();
    private List<int> levelList = new List<int>();

    public Dictionary<int, HeroAbilitySystem> allAbilityDic = new Dictionary<int, HeroAbilitySystem>();

    private HeroStatusInfo statusInfo;

    public bool isHealthUI = false;

    float rand;
    float rand2;
    float rand3;
    float rand4;

    private void Start()
    {
        SetWave().Forget();
    }

    private async UniTaskVoid SetWave()
    {
        while (true) // 게임 끝나기 전까지
        {
            await UniTask.Delay(TimeSpan.FromSeconds(10)); //매 10초마다

            SetNextHero();
            SummonHero();
        }
    }

    private void SetNextHero()
    {
        int cnt = DataManager.Instance.heroStatusDic.Count;

        int rand = UnityEngine.Random.Range(0, cnt);


        statusInfo = DataManager.Instance.heroStatusDic.ElementAt(rand).Value;



    }


    private void SummonHero()
    {
        HeroController hero = HeroPoolManager.Instance.GetObject(RandomSummonPos(98, 98));
        hero?.StatInit(statusInfo, HeroManager.Instance.isHealthUI);
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
