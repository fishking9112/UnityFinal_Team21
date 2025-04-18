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
    public HeroAbilitySystem a;

    private int id = 101;
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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            GameObject hero = HeroPoolManager.Instance.GetObject(Vector2.zero);
            hero.GetComponent<HeroController>().InitAbility(idList, levelList);
        }
    }
}
