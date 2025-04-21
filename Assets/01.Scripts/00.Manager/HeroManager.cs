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
    float rand;
    float rand2;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            rand = UnityEngine.Random.Range(-45, 45);
            rand2 = UnityEngine.Random.Range(-45, 45);
            idList.Clear();
            levelList.Clear();
            //var l = Enumerable.Range(101, 5).OrderBy(x => UnityEngine.Random.value).Take(3);

            //foreach(int i in l)
            //{
            //    idList.Add(i);
            //    Utils.Log(i.ToString());
            //}

            //var lv= Enumerable.Range(1,8).OrderBy(x => UnityEngine.Random.value).Take(3);

            //foreach (int i in lv)
            //{
            //    levelList.Add(i);
            //    Utils.Log(i.ToString());
            //}

            idList.Add(101);
            levelList.Add(1);
            idList.Add(102);
            levelList.Add(1);
            idList.Add(103);
            levelList.Add(1);
            idList.Add(104);
            levelList.Add(1);
            idList.Add(105);
            levelList.Add(1);
            HeroController hero = HeroPoolManager.Instance.GetObject(new Vector2(rand, rand2));
            hero.InitAbility(idList, levelList);
        }
    }

    private async UniTaskVoid SetWave()
    {
        while(true) // 게임 끝나기 전까지
        {
            await UniTask.Delay(TimeSpan.FromSeconds(3)); //매 3초마다

            SetNextHero();
            SummonHero();
        }
    }

    private void SetNextHero()
    {
        idList.Clear();
        levelList.Clear();

        idList.Add(101);
        levelList.Add(1);
        idList.Add(102);
        levelList.Add(1);
        idList.Add(103);
        levelList.Add(1);
        idList.Add(104);
        levelList.Add(1);
        idList.Add(105);
        levelList.Add(1);
    }


    private void SummonHero()
    {
        HeroController hero = HeroPoolManager.Instance.GetObject(new Vector2(rand, rand2));
        hero.InitAbility(idList, levelList);
    }

}
