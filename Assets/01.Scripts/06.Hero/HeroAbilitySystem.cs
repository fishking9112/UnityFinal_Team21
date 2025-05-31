using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;


public abstract class HeroAbilitySystem : MonoBehaviour
{
    private HeroAbilityInfo heroAbilityInfo;
    private HeroAbilityLevelUpInfo heroAbilityLevelUpInfo;

    protected GameObject target;

    [Header("Base Stat")]
    [SerializeField] protected int id;
    [SerializeField] protected int maxLevel;
    [SerializeField] protected Vector3 pivot;
    [SerializeField] protected float damage;
    [SerializeField] protected float damage_LevelUp;
    [SerializeField] protected float delay;
    [SerializeField] protected float delay_LevelUp;
    [SerializeField] protected float pierce;
    [SerializeField] protected float pierce_LevelUp;
    [SerializeField] protected Vector3 size;
    [SerializeField] protected Vector3 size_LevelUp;
    [SerializeField] protected HeroAbilityType type;
    [SerializeField] protected float speed;
    [SerializeField] protected float speed_LevelUp;
    [SerializeField] protected float rotateSpeed;
    [SerializeField] protected float rotateSpeed_LevelUp;
    [SerializeField] protected float duration;
    [SerializeField] protected float duration_LevelUp;
    [SerializeField] protected float count;
    [SerializeField] protected float count_LevelUp;
    [SerializeField] protected float countDelay;
    [SerializeField] protected float countDelay_LevelUp;
    [SerializeField] protected float knockback;
    [SerializeField] protected float damage_Delay;
    [SerializeField] protected float damage_Range;

    [SerializeField] protected int curLevel;


    protected CancellationTokenSource token;

    

    public virtual void Initialize(int id)
    {
        heroAbilityInfo = DataManager.Instance.heroAbilityDic[id];

        heroAbilityLevelUpInfo = DataManager.Instance.heroAbilityLevelUpDic[heroAbilityInfo.levelUp_ID];

        Init();
    }

    private void Init()
    {
        id = heroAbilityInfo.id;
        delay = heroAbilityInfo.delay_Base;
        delay_LevelUp = heroAbilityLevelUpInfo.delay_LevelUp;
        damage = heroAbilityInfo.damage_Base;
        damage_LevelUp = heroAbilityLevelUpInfo.damage_LevelUp;
        pierce = heroAbilityInfo.piercing_Base;
        pierce_LevelUp = heroAbilityLevelUpInfo.piercing_LevelUp;
        size = heroAbilityInfo.size_Base;
        size_LevelUp = heroAbilityLevelUpInfo.size_LevelUp;
        type = heroAbilityInfo.type;
        speed = heroAbilityInfo.speed_Base;
        speed_LevelUp = heroAbilityLevelUpInfo.speed_LevelUp;
        rotateSpeed = heroAbilityInfo.rotateSpeed_Base;
        rotateSpeed_LevelUp = heroAbilityLevelUpInfo.rotateSpeed_LevelUp;
        duration = heroAbilityInfo.duration_Base;
        duration_LevelUp = heroAbilityLevelUpInfo.duration_LevelUp;
        count = heroAbilityInfo.count_Base;
        count_LevelUp = heroAbilityLevelUpInfo.count_LevelUp;
        countDelay = heroAbilityInfo.countDelay_Base;
        countDelay_LevelUp = heroAbilityLevelUpInfo.countDelay_LevelUp;
        knockback = heroAbilityInfo.knockback;
        damage_Delay = heroAbilityInfo.damage_Delay;
        damage_Range= heroAbilityInfo.damage_Range;
        maxLevel = heroAbilityInfo.maxLevel;
        pivot = heroAbilityInfo.pivot;
        curLevel = 1;
    }

    /// <summary>
    /// 능력 획득
    /// </summary>
    protected virtual void AddAbility()
    {

        var destroyToken = this.GetCancellationTokenOnDestroy();

        token = new CancellationTokenSource();
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(destroyToken, token.Token);

        var linkedToken= linked.Token;
        AutoAction(linkedToken).Forget();
    }

    /// <summary>
    /// delayTime간격으로 ActionAbility 호출
    /// </summary>
    /// <returns></returns>
    protected async UniTaskVoid AutoAction(CancellationToken tk)
    {
        try
        {
            while (!tk.IsCancellationRequested && this.enabled)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delay), false, PlayerLoopTiming.Update, cancellationToken: tk);

                if (tk.IsCancellationRequested || !this.enabled)
                {
                    break;
                }

                ActionAbility();
            }
        }
        catch (MissingReferenceException)
        {
            // 무시해도 되는 예외
        }

    }

    /// <summary>
    /// 실제 하는 행동 호출
    /// </summary>
    protected abstract void ActionAbility();

    public virtual void AbilityLevelUp()
    {
        if (maxLevel == curLevel)
        {
            return;
        }

        curLevel++;
        damage += damage_LevelUp;
        delay -= delay_LevelUp;
        pierce += pierce_LevelUp;
        size += size_LevelUp;
        speed += speed_LevelUp;
        rotateSpeed += rotateSpeed_LevelUp;
        duration += duration_LevelUp;
        count += count_LevelUp;
        countDelay -= countDelay_LevelUp;
    }

    public abstract void DespawnAbility();

    public virtual void SetAbilityLevel(int level)
    {
        Init();

        for (int i = 1; i < level; i++)
        {
            AbilityLevelUp();
        }
        AddAbility();
    }

    public int GetID()
    {
        return id;
    }
}
