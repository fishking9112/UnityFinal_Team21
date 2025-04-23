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

    float rand;
    float rand2;

    private void Start()
    {
        SetWave().Forget();
    }

    private async UniTaskVoid SetWave()
    {
        while(true) // 게임 끝나기 전까지
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
        rand = UnityEngine.Random.Range(-45, 45);
        rand2 = UnityEngine.Random.Range(-45, 45);

        HeroController hero = HeroPoolManager.Instance.GetObject(new Vector2(rand, rand2));
        hero.StatInit(statusInfo);
    }

}
