using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public GameObject target;

    public List<HeroAbilitySystem> abilityList;

    public List<HeroAbilitySystem> allAbility;

    private LayerMask mask;

    private float detectedRange;

    public void Init(float range)
    {
        abilityList.Clear();
        allAbility.Clear();
        abilityList = new List<HeroAbilitySystem>();
        allAbility = GetComponents<HeroAbilitySystem>().ToList();
        detectedRange = range;
        mask = 1 << 7 | 1 << 13;
    }

    public GameObject FindNearestTarget()
    {
        target = null;

        Collider2D[] col = Physics2D.OverlapCircleAll (transform.position, detectedRange, 1 << 7|1<<13);
        if (col.Length==0)
        {
            return target;
        }

        float minVal = float.MaxValue;
        float distance;
        foreach (Collider2D c in col)
        {
            distance = Vector2.Distance(c.transform.position, transform.position);
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
            break;
        }
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
