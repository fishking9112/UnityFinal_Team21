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

    int weapon1;
    int weapon2;
    int weapon3;

    int level1;
    int level2;
    int level3;


    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
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
            idList.Add(102);
            levelList.Add(1);

            GameObject hero = HeroPoolManager.Instance.GetObject(new Vector2(rand,rand2));
            hero.GetComponent<HeroController>().InitAbility(idList, levelList);
        }
    }
}
