using UnityEngine;
using Cysharp.Threading.Tasks;
using System;


public abstract class HeroAbilitySystem : MonoBehaviour
{
    public HeroAbilityInfo heroAbilityInfo;

    protected GameObject target;

    [Header("Base Stat")]
    [SerializeField] protected float delay;
    [SerializeField] protected float damage;
    [SerializeField] protected float knockback;
    [SerializeField] protected int maxLevel;
    [SerializeField] protected Vector3 pivot;
    [SerializeField] protected Vector3 size;
    [SerializeField] protected int curLevel;

    protected virtual void Start()
    {
        Init();
    }

    private void Init()
    {
        delay = heroAbilityInfo.delay_Base;
        damage = heroAbilityInfo.damage_Base;
        knockback = 1; // 임시 값
        maxLevel = heroAbilityInfo.maxLevel;
        pivot = heroAbilityInfo.pivot;
        size = heroAbilityInfo.size_Base;
        curLevel = 1;
    }

    /// <summary>
    /// 능력 획득
    /// </summary>
    protected virtual void AddAbility()
    {
        AutoAction().Forget();
    }


    /// <summary>
    /// delayTime간격으로 ActionAbility 호출
    /// </summary>
    /// <returns></returns>
    protected async UniTaskVoid AutoAction()
    {

        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            ActionAbility();
        }

    }

    /// <summary>
    /// 실제 하는 행동 호출
    /// </summary>
    protected abstract void ActionAbility();

    public virtual void AbilityLevelUp()
    {
        if(maxLevel == curLevel)
        {
            return;
        }

        curLevel++;
        damage += heroAbilityInfo.damage_LevelUp;
        delay -= heroAbilityInfo.delay_LevelUp;
        size += heroAbilityInfo.size_LevelUp;
    }

    public abstract void DespawnAbility();
}
