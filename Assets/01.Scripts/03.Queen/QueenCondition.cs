using Cysharp.Threading.Tasks;
using System.Collections;
using UnityEngine;

public class QueenCondition : MonoBehaviour
{
    private QueenStatusInfo queenStatus;

    private float initCurExpGauge = 0f;
    private float initMaxExpGauge = 15f;
    private float initEvolutionPoint = 0f;
    private float initLevel = 1f;
    private float initGold = 0f;
    private int initEnhnacePoint = 0;

    private float expGainMultiplierPercent = 0f;
    private float goldGainMultiplierPercent = 0f;

    public float SummonGaugeRecoverySpeed { get; private set; }
    public float QueenActiveSkillGaugeRecoverySpeed { get; private set; }
    public float AbilityUpgrade_SummonGaugeRecoverySpeed { get; private set; }
    public float AbilityUpgrade_MaxSummonGauge { get; private set; }
    public float AbilityUpgrade_QueenActiveSkillGaugeRecoverySpeed { get; private set; }
    public float AbilityUpgrade_MaxQueenActiveSkillGauge { get; private set; }
    public ReactiveProperty<float> CurQueenActiveSkillGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> MaxQueenActiveSkillGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> CurSummonGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> MaxSummonGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> CurExpGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> MaxExpGauge { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> EvolutionPoint { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> Level { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<float> Gold { get; private set; } = new ReactiveProperty<float>();
    public ReactiveProperty<int> KillCnt { get; private set; } = new ReactiveProperty<int>();
    public int EnhancePoint;

    private float ExpGainMultiplier => 1f + (expGainMultiplierPercent * 0.01f);
    private float GoldGainMultiplier => 1f + (goldGainMultiplierPercent * 0.01f);

    private int levelUpCount = 0;
    private bool isLevelUpDoing = false;
    public bool InitComplete = false;

    private void Awake()
    {
        Level.Value = initLevel;
        CurExpGauge.Value = initCurExpGauge;
        MaxExpGauge.Value = initMaxExpGauge;
        EvolutionPoint.Value = initEvolutionPoint;
        Gold.Value = initGold;
        EnhancePoint = initEnhnacePoint;
    }

    private async void Start()
    {
        await InitQueenStatus();
        await InitSkill();

        InitComplete = true;
    }

    private async UniTask InitQueenStatus()
    {
        await UniTask.WaitUntil(() => DataManager.Instance.queenStatusDic.ContainsKey(GameManager.Instance.QueenCharaterID));

        queenStatus = DataManager.Instance.queenStatusDic[GameManager.Instance.QueenCharaterID];

        SummonGaugeRecoverySpeed = queenStatus.summon_Recorvery;
        QueenActiveSkillGaugeRecoverySpeed = queenStatus.mana_Recorvery;
        CurQueenActiveSkillGauge.Value = queenStatus.mana_Base;
        MaxQueenActiveSkillGauge.Value = queenStatus.mana_Base;
        CurSummonGauge.Value = queenStatus.summon_Base;
        MaxSummonGauge.Value = queenStatus.summon_Base;
    }

    private async UniTask InitSkill()
    {
        await UniTask.WaitUntil(() => QueenActiveSkillManager.Instance.queenActiveSkillDic != null &&
                                      QueenPassiveSkillManager.Instance.queenPassiveSkillDic != null);

        QueenActiveSkillManager.Instance.AddSkill(0, queenStatus.baseActiveSkill);
        QueenPassiveSkillManager.Instance.AddSkill(queenStatus.basePassiveSkill_1);
        QueenPassiveSkillManager.Instance.AddSkill(queenStatus.basePassiveSkill_2);
        QueenPassiveSkillManager.Instance.AddSkill(queenStatus.basePassiveSkill_3);
    }

    /// <summary>
    /// 현재 액티브 스킬 게이지 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustCurQueenActiveSkillGauge(float amount)
    {
        CurQueenActiveSkillGauge.Value = AdjustValue(CurQueenActiveSkillGauge.Value, amount, MaxQueenActiveSkillGauge.Value);
    }

    /// <summary>
    /// 최대 액티브 스킬 게이지 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustMaxQueenActiveSkillGauge(float amount)
    {
        MaxQueenActiveSkillGauge.Value = AdjustValue(MaxQueenActiveSkillGauge.Value, amount, float.MaxValue);
        CurQueenActiveSkillGauge.Value = AdjustValue(CurQueenActiveSkillGauge.Value, amount, MaxQueenActiveSkillGauge.Value);
    }

    /// <summary>
    /// 현재 소환 게이지 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustCurSummonGauge(float amount)
    {
        CurSummonGauge.Value = AdjustValue(CurSummonGauge.Value, amount, MaxSummonGauge.Value);
    }

    /// <summary>
    /// 최대 소환 게이지 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustMaxSummonGauge(float amount)
    {
        MaxSummonGauge.Value = AdjustValue(MaxSummonGauge.Value, amount, float.MaxValue);
        CurSummonGauge.Value = AdjustValue(CurSummonGauge.Value, amount, MaxSummonGauge.Value);
    }

    /// <summary>
    /// 소환 게이지 회복 속도 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustSummonGaugeRecoverySpeed(float amount)
    {
        SummonGaugeRecoverySpeed = AdjustValue(SummonGaugeRecoverySpeed, amount, float.MaxValue);
    }

    /// <summary>
    /// 마나 게이지 회복 속도 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustQueenActiveSkillGaugeRecoverySpeed(float amount)
    {
        QueenActiveSkillGaugeRecoverySpeed = AdjustValue(QueenActiveSkillGaugeRecoverySpeed, amount, float.MaxValue);
    }

    /// <summary>
    /// 경험치 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustCurExpGauge(float amount)
    {
        float adjustedAmount = amount * ExpGainMultiplier;
        float temp = CurExpGauge.Value + adjustedAmount;

        while (temp >= MaxExpGauge.Value)
        {
            levelUpCount++;
            temp -= MaxExpGauge.Value;
        }

        CurExpGauge.Value = temp;
        StartCoroutine(CoroutineLevelUp());
    }

    /// <summary>
    /// 레벨업 처리 및 강화 트리거 호출
    /// </summary>
    private void LevelUp()
    {
        Level.Value++;
        EnhancePoint++;
        ExpIncrease();
        if (Level.Value % 5 == 0)
        {
            AdjustEvolutionPoint(1);
        }
        AdjustMaxQueenActiveSkillGauge(queenStatus.mana_LevelUp);
        AdjustMaxSummonGauge(queenStatus.summon_LevelUp);

        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().ShowWindow<QueenEnhanceUI>();
    }

    private void ExpIncrease()
    {
        float stepBonus = 50 * (Level.Value / 10);
        MaxExpGauge.Value = initMaxExpGauge * Mathf.Pow(Level.Value, 1.5f) + stepBonus;
    }

    public IEnumerator CoroutineLevelUp()
    {
        if (isLevelUpDoing) yield break;
        isLevelUpDoing = true;

        while (levelUpCount > 0)
        {
            LevelUp();
            yield return new WaitUntil(() => !StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().queenEnhanceUI.isOpen);
            levelUpCount--;
        }

        isLevelUpDoing = false;
    }

    /// <summary>
    /// 진화 포인트 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustEvolutionPoint(float amount)
    {
        EvolutionPoint.Value = AdjustValue(EvolutionPoint.Value, amount, float.MaxValue);
    }

    /// <summary>
    /// 골드 조정
    /// </summary>
    /// <param name="amount"> 조정할 수치 </param>
    public void AdjustGold(float amount)
    {
        float adjustedAmount = amount * GoldGainMultiplier;
        Gold.Value = AdjustValue(Gold.Value, adjustedAmount, float.MaxValue);
    }

    /// <summary>
    /// 경험치 획득량 증가 비율 설정 (% 단위)
    /// </summary>
    public void SetExpGainMultiplierPercent(float percent)
    {
        expGainMultiplierPercent = Mathf.Max(0f, percent);
    }

    /// <summary>
    /// 골드 획득량 증가 비율 설정 (% 단위)
    /// </summary>
    public void SetGoldGainMultiplierPercent(float percent)
    {
        goldGainMultiplierPercent = Mathf.Max(0f, percent);
    }

    // Ability 적용 함수
    public void AbilitySummonGaugeRecoverySpeed(float percent)
    {
        AbilityUpgrade_SummonGaugeRecoverySpeed = AdjustValueByPercent(queenStatus.summon_Recorvery, percent, float.MaxValue);
        SummonGaugeRecoverySpeed += AbilityUpgrade_SummonGaugeRecoverySpeed;
    }
    public void AbilityMaxSummonGauge(float percent)
    {
        AbilityUpgrade_MaxSummonGauge = AdjustValueByPercent(queenStatus.summon_Base, percent, float.MaxValue);
        MaxSummonGauge.Value += AbilityUpgrade_MaxSummonGauge;
        CurSummonGauge.Value += AbilityUpgrade_MaxSummonGauge;
    }
    public void AbilityQueenActiveSkillGaugeRecoverySpeed(float percent)
    {
        AbilityUpgrade_QueenActiveSkillGaugeRecoverySpeed = AdjustValueByPercent(queenStatus.mana_Recorvery, percent, float.MaxValue);
        QueenActiveSkillGaugeRecoverySpeed += AbilityUpgrade_QueenActiveSkillGaugeRecoverySpeed;
    }
    public void AbilityMaxQueenActiveSkillGauge(float percent)
    {
        AbilityUpgrade_MaxQueenActiveSkillGauge = AdjustValueByPercent(queenStatus.mana_Base, percent, float.MaxValue);
        MaxQueenActiveSkillGauge.Value += AbilityUpgrade_MaxQueenActiveSkillGauge;
        CurQueenActiveSkillGauge.Value += AbilityUpgrade_MaxQueenActiveSkillGauge;
    }

    // 값 조정
    private float AdjustValue(float cur, float amount, float max)
    {
        return Mathf.Clamp(cur + amount, 0f, max);
    }

    private float AdjustValueByPercent(float cur, float percent, float max)
    {
        return Mathf.Clamp(cur * percent, 0f, max);
    }
}