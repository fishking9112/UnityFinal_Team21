using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public GameObject target;

    private int enemyLayer;

    public List<HeroAbilitySystem> abilityList;

    public List<HeroAbilitySystem> allAbility;

    private void Start()
    {


    }

    public void Init()
    {
        abilityList.Clear();
        allAbility.Clear();

        abilityList = new List<HeroAbilitySystem>();
        allAbility = GetComponents<HeroAbilitySystem>().ToList();

    }

    public GameObject FindNearestTarget()
    {
        target = null;

        Vector2 pointA = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector2 pointB = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height)); ;

        Vector2 off = pointA - pointB;

        pointA = (Vector2)transform.position - off / 2;
        pointB = (Vector2)transform.position + off / 2;

        Collider2D[] col = Physics2D.OverlapAreaAll(pointA, pointB, 1<<7);

        if (col.Length == 0)
            return null;

        float minVal = float.MaxValue;

        foreach (Collider2D c in col)
        {
            float distance = Vector2.Distance(c.transform.position, transform.position);
            if (minVal > distance)
            {
                minVal = distance;
                target = c.gameObject;
            }
        }

        return target;
    }



    /// <summary>
    /// 히어로 능력 추가
    /// </summary>
    /// <typeparam name="T"> 넣고 싶은 능력 클래스 이름 </typeparam>
    public void AddAbility(int id) 
    {
        foreach (var a in allAbility)
        {
            if (a.GetID() != id)
            {
                continue;
            }
            a.enabled = true;
            abilityList.Add(a);
        }

    }

    public void SetAbilityLevel(int id, int level)
    {
        foreach(var a in allAbility)
        {
            if(a.GetID()!=id)
            {
                continue;
            }
            a.enabled = true;
            abilityList.Add(a);

            a.SetAbilityLevel(level);
        }


        //if (HeroManager.Instance.allAbilityDic.TryGetValue(id, out HeroAbilitySystem ability))
        //{
        //    if (!abilityList.Contains(ability))
        //    {
        //        ability.enabled = true;
        //        abilityList.Add(ability);
        //    }
        //    ability.SetAbilityLevel(level);
        //}
    }

    /// <summary>
    /// 히어로 능력 제거
    /// </summary>
    /// <typeparam name="T"> 제거 하고 싶은 능력 클래스 이름 </typeparam>
    public void RemoveAbility(int id) 
    {
        foreach (var a in abilityList)
        {
            if (a.GetID() != id)
            {
                continue;
            }
            a.enabled = false;
            a.DespawnAbility();
            abilityList.Remove(a);
        }

        //if (HeroManager.Instance.allAbilityDic.TryGetValue(id, out HeroAbilitySystem ability))
        //{
        //    if (abilityList.Contains(ability))
        //    {
        //        ability.enabled = false;
        //        ability.DespawnAbility();
        //        abilityList.Remove(ability);
        //    }
        //}
    }

    /// <summary>
    /// 히어로 능력 초기화
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void ResetAbility()
    {
        foreach (var ability in abilityList)
        {
            ability.enabled = false;
            ability.DespawnAbility();
        }

        abilityList.Clear();
    }
}
