using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class QueenAbilityUpgradeInfo
{
    public int id;
    public int level;
}

public class QueenAbilityUpgradeManager : MonoSingleton<QueenAbilityUpgradeManager>
{
    [SerializeField] private QueenAbilityUpgradeItem abilityItemPrefab;

    private QueenAbilityUIController queenAbilityUIController;
    public QueenAbilityUIController QueenAbilityUIController => queenAbilityUIController;
    private Dictionary<int, int> upgradeLevels = new();
    

    private readonly Dictionary<int, QueenAbilityUpgradeItem> abilityItemDict = new();

    private Dictionary<int, Action<float>> applyEffectActions;



    protected override void Awake()
    {
        base.Awake();
        Initialize();
        InitializeEffectActions();
    }

    #region 초기화

    /// <summary>
    /// 능력 효과에 대한 델리게이트를 초기화합니다.
    /// </summary>
    private void InitializeEffectActions()
    {
        applyEffectActions = new Dictionary<int, Action<float>>();

        Action<float>[] actions =
        {
            value => ApplyAttackPowerBuff(value),
            value => ApplyMoveSpeedBuff(value),
            value => GameManager.Instance.queen.condition.SetGoldGainMultiplierPercent(value),
            value => GameManager.Instance.queen.condition.SetExpGainMultiplierPercent(value),
            value => GameManager.Instance.castle.condition.AdjustMaxHealth(value),
            value => GameManager.Instance.castle.condition.AdjustCastleHealthRecoverySpeed(value),
            value => GameManager.Instance.queen.condition.AdjustSummonGaugeRecoverySpeed(value),
            value => GameManager.Instance.queen.condition.AdjustMaxSummonGauge(value),
            value => GameManager.Instance.queen.condition.AdjustQueenActiveSkillGaugeRecoverySpeed(value),
            value => GameManager.Instance.queen.condition.AdjustMaxQueenActiveSkillGauge(value),
            value => GameManager.Instance.queen.condition.AdjustEvolutionPoint(value)
        };

        int index = 0;
        foreach (var kvp in DataManager.Instance.queenAbilityDic)
        {
            if (index >= actions.Length)
            {
                Utils.LogWarning("Action보다 queenAbilityDic의 항목 수가 더 많음");
                break;
            }

            applyEffectActions[kvp.Key] = actions[index];
            index++;
        }
    }

    /// <summary>
    /// 업그레이드 레벨 정보를 초기화합니다.
    /// </summary>
    public void Initialize()
    {
        if (DataManager.Instance.queenAbilityDic == null)
        {
            Utils.LogError("DataManager의 queenAbilityDic 없음");
            return;
        }

        upgradeLevels.Clear();

        foreach (var kvp in DataManager.Instance.queenAbilityDic)
        {
            upgradeLevels[kvp.Key] = 0;
        }
    }

    #endregion

    #region 강화 시도

    /// <summary>
    /// 능력 업그레이드를 시도합니다.
    /// </summary>
    public void TryUpgrade(int id)
    {
        if (!IsValidAbility(id, out var ability)) return;

        int currentLevel = upgradeLevels[id];
        if (currentLevel >= ability.maxLevel) return;

        int cost = ability.levelInfo[currentLevel].cost;
        if (!GameManager.Instance.TrySpendGold(cost))
        {
            Utils.Log("골드 부족으로 업그레이드 실패");
            return;
        }

        upgradeLevels[id]++;
        queenAbilityUIController.SetPopupQueenAbilityInfo(ability, upgradeLevels[id]);


    }

    /// <summary>
    /// 능력 다운그레이드를 시도합니다.
    /// </summary>
    public void TryDowngrade(int id)
    {
        if (!IsValidAbility(id, out var ability)) return;

        int currentLevel = upgradeLevels[id];
        if (currentLevel <= 0) return;

        int refund = ability.levelInfo[currentLevel - 1].cost;
        RefundCurrency(refund);

        upgradeLevels[id]--;
        queenAbilityUIController.SetPopupQueenAbilityInfo(ability, upgradeLevels[id]);

    }

    #endregion

    #region 효과 적용 

    /// <summary>
    /// 저장된 강화 수치를 기반으로 모든 강화 효과를 적용합니다.
    /// </summary>
    public void ApplyAllEffects()
    {
        foreach (var kvp in upgradeLevels)
        {
            int id = kvp.Key;
            int level = kvp.Value;

            if (level <= 0 || !applyEffectActions.ContainsKey(id)) continue;

            var ability = GetAbilityById(id);
            if (ability == null) continue;

            float value = ability.levelInfo[level - 1].eff;
            applyEffectActions[id].Invoke(value);
        }
    }

    public void ResetQueenAbilityMonsterValues()
    {
        // 공격력 감소
        if (upgradeLevels.TryGetValue(0, out int atkLevel) && atkLevel > 0)
        {
            var atkAbility = GetAbilityById(0);
            float atkValue = atkAbility.levelInfo[atkLevel - 1].eff;
            ApplyAttackPowerBuff(-atkValue);
        }

        // 이동속도 감소
        if (upgradeLevels.TryGetValue(1, out int speedLevel) && speedLevel > 0)
        {
            var speedAbility = GetAbilityById(1);
            float speedValue = speedAbility.levelInfo[speedLevel - 1].eff;
            ApplyMoveSpeedBuff(-speedValue);
        }

    }

    private void ApplyAttackPowerBuff(float value)
    {
        foreach (var kvp in DataManager.Instance.monsterDic)
        {
            kvp.Value.attack += value;
        }
    }

    private void ApplyMoveSpeedBuff(float value)
    {
        foreach (var kvp in DataManager.Instance.monsterDic)
        {
            kvp.Value.moveSpeed += value;
        }
    }
    #endregion

    #region 리셋

    /// <summary>
    /// 모든 능력을 초기화하고, 소모된 재화를 반환합니다.
    /// </summary>
    public void ResetAllAbilities()
    {
        int totalRefundCost = 0;

        foreach (var kvp in DataManager.Instance.queenAbilityDic)
        {
            var ability = kvp.Value;
            int id = ability.ID;
            int currentLevel = upgradeLevels[id];

            for (int i = 0; i < currentLevel; i++)
            {
                totalRefundCost += ability.levelInfo[i].cost;
            }

            upgradeLevels[id] = 0;
        }

        RefreshAllAbilityItems();
        RefundCurrency(totalRefundCost);

        Utils.Log($"모든 능력을 초기화하고 {totalRefundCost}만큼 재화를 반환받음.");
    }

    #endregion

    #region 저장, 불러오기

    /// <summary>
    /// 현재 상태를 저장 데이터 형태로 변환합니다.
    /// </summary>
    public QueenAbilityUpgradeData SetSaveData()
    {
        var saveData = new QueenAbilityUpgradeData
        {
            upgrades = new List<QueenAbilityUpgradeInfo>()
        };

        foreach (var pair in upgradeLevels)
        {
            saveData.upgrades.Add(new QueenAbilityUpgradeInfo
            {
                id = pair.Key,
                level = pair.Value
            });
        }

        return saveData;
    }

    /// <summary>
    /// 저장된 데이터를 불러와 적용합니다.
    /// </summary>
    public void ApplyUpgradeData(QueenAbilityUpgradeData data)
    {
        if (data.upgrades == null)
        {
            Utils.LogWarning("적용할 여왕 강화 데이터가 없음");
            return;
        }

        foreach (var info in data.upgrades)
        {
            if (upgradeLevels.ContainsKey(info.id))
            {
                var ability = GetAbilityById(info.id);
                if (ability != null && info.level <= ability.maxLevel)
                {
                    upgradeLevels[info.id] = info.level;
                }
            }
        }

        RefreshAllAbilityItems();

    }

    #endregion


    #region UI 연동

    /// <summary>
    /// UI 컨트롤러를 설정하고 아이템을 생성합니다. 메인메뉴씬 들어올때마다 ui컨트롤러에서 실행
    /// </summary>
    public void SetQueenAbilityUIController(QueenAbilityUIController script)
    {
        queenAbilityUIController = script;
        CreateAbilityItems();
    }

    /// <summary>
    /// 능력 UI 아이템을 생성합니다.
    /// </summary>
    public void CreateAbilityItems()
    {
        abilityItemDict.Clear();

        foreach (var kvp in DataManager.Instance.queenAbilityDic)
        {
            var ability = kvp.Value;
            var item = Instantiate(abilityItemPrefab, QueenAbilityUIController.ContentTransform);
            item.Initialize(ability, GetLevel(ability.ID));
            abilityItemDict[ability.ID] = item;
        }
    }

    /// <summary>
    /// 모든 능력 아이템 UI를 갱신합니다.
    /// </summary>
    public void RefreshAllAbilityItems()
    {
        foreach (var kvp in abilityItemDict)
        {
            int id = kvp.Key;
            kvp.Value.Refresh(GetLevel(id));
        }
    }

    #endregion

    #region 보조 

    private void RefundCurrency(int amount)
    {
        GameManager.Instance.AddGold(amount);
    }

    private bool IsValidAbility(int id, out QueenAbilityInfo ability)
    {
        ability = GetAbilityById(id);
        return upgradeLevels.ContainsKey(id) && ability != null;
    }

    private QueenAbilityInfo GetAbilityById(int id)
    {
        DataManager.Instance.queenAbilityDic.TryGetValue(id, out var ability);
        return ability;
    }

    public int GetLevel(int id)
    {
        return upgradeLevels.TryGetValue(id, out int level) ? level : 0;
    }

    public int GetEffectValue(int id)
    {
        if (!upgradeLevels.TryGetValue(id, out int level) || level <= 0)
            return 0;

        var ability = GetAbilityById(id);
        return ability?.levelInfo[level - 1].eff ?? 0;
    }

    #endregion

}
