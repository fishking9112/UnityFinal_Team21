using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour, IPoolable
{
    private HeroState stateMachine;

    public  HeroController controller;
    public GameObject target;

    private int enemyLayer;

    public List<HeroAbilitySystem> abilityList;
    private Dictionary<Type, HeroAbilitySystem> allAbilityDic;

    private Action<GameObject> returnToPool;

    private void Awake()
    {
        GameManager.Instance.hero = this;
    }

    private void Start()
    {
        stateMachine = new HeroState(this);
        controller=GetComponent<HeroController>();
        stateMachine.navMeshAgent = controller.navMeshAgent;
        stateMachine.ChangeState(stateMachine.moveState);

        enemyLayer = LayerMask.GetMask("Monster");

        DeadCheck().Forget();

        abilityList = new List<HeroAbilitySystem>();
        allAbilityDic = new Dictionary<Type, HeroAbilitySystem>();

        foreach (var ability in GetComponents<HeroAbilitySystem>())
        {
            Type type = ability.GetType();
            ability.enabled = false;
            allAbilityDic[type] = ability;
        }

        // 테스트 코드(성경책, 미사일 추가)
        AddAbility<HeroAbilityBible>();
        AddAbility<HeroAbilityMissile>();
    }

    public GameObject FindNearestTarget()
    {
        target = null;

        Vector2 pointA = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector2 pointB = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height)); ;

        Vector2 off = pointA - pointB;

        pointA = (Vector2)transform.position - off / 2;
        pointB = (Vector2)transform.position + off / 2;

        Collider2D[] col = Physics2D.OverlapAreaAll(pointA, pointB, enemyLayer);

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

        Debug.Log(target);

        return target;
    }

    public void Init(Action<GameObject> returnAction)
    {
        returnToPool += returnAction;
    }

    public void OnSpawn()
    {
        stateMachine = new HeroState(this);
        stateMachine.ChangeState(stateMachine.moveState);

        enemyLayer = LayerMask.GetMask("Monster");

        DeadCheck().Forget();
    }

    private async UniTaskVoid DeadCheck()
    {
        // 사망 체크로 수정 핋요
        await UniTask.WaitUntil(() => gameObject.activeSelf == false);
        stateMachine.ChangeState(stateMachine.deadState);
    }

    public void OnDespawn()
    {
        returnToPool?.Invoke(gameObject);
    }

    /// <summary>
    /// 히어로 능력 추가
    /// </summary>
    /// <typeparam name="T"> 넣고 싶은 능력 클래스 이름 </typeparam>
    public void AddAbility<T>() where T : HeroAbilitySystem
    {
        Type type = typeof(T);

        if (allAbilityDic.TryGetValue(type, out HeroAbilitySystem ability))
        {
            if (!abilityList.Contains(ability))
            {
                ability.enabled = true;
                abilityList.Add(ability);
            }
        }
        else
        {
            Utils.Log($"{type}은 존재하지 않습니다.");
        }
    }

    /// <summary>
    /// 히어로 능력 제거
    /// </summary>
    /// <typeparam name="T"> 제거 하고 싶은 능력 클래스 이름 </typeparam>
    public void RemoveAbility<T>() where T : HeroAbilitySystem
    {
        Type type = typeof(T);

        if (allAbilityDic.TryGetValue(type, out HeroAbilitySystem ability))
        {
            if (abilityList.Contains(ability))
            {
                ability.enabled = false;
                ability.DespawnAbility();
                abilityList.Remove(ability);
            }
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
