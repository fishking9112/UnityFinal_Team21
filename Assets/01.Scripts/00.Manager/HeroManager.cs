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
        idList.Add(103);
        idList.Add(102);
        levelList.Add(3);
        levelList.Add(1);


    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            idList.Clear();
            levelList.Clear();

            rand = UnityEngine.Random.Range(-45, 45);
            rand2 = UnityEngine.Random.Range(-45, 45);
            
            weapon1 = UnityEngine.Random.Range(101, 106);
            weapon2 = UnityEngine.Random.Range(101, 106);
            weapon3 = UnityEngine.Random.Range(101, 106);

            level1 = UnityEngine.Random.Range(1, 9);
            level2 = UnityEngine.Random.Range(1, 9);
            level3 = UnityEngine.Random.Range(1, 9);

            idList.Add(weapon1);
            idList.Add(weapon2);
            idList.Add(weapon3);

            levelList.Add(level1);
            levelList.Add(level2);
            levelList.Add(level3);

            GameObject hero = HeroPoolManager.Instance.GetObject(new Vector2(rand,rand2));
            hero.GetComponent<HeroController>().InitAbility(idList, levelList);
        }
    }
}
