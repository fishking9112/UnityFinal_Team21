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

    private readonly Dictionary<int, QueenAbilityUpgradeItem> abilityItemDict = new();
    public IReadOnlyDictionary<int, QueenAbilityUpgradeItem> AbilityItemDict => abilityItemDict;


    private Dictionary<int, int> upgradeLevels = new();
    private QueenAbilityPanelUI queenAbilityPanelUI;
    public QueenAbilityPanelUI QueenAbilityPanelUI => queenAbilityPanelUI;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    /// <summary>
    /// 업그레이드 데이터를 초기화합니다.
    /// 게임 시작 시 호출되며, 각 능력의 레벨을 0으로 설정합니다.
    /// </summary>
    public void Initialize()
    {
        if (DataManager.Instance.queenAbilityDic == null)
        {
            Utils.LogError("DataManager의 queenAilityDic 없음");
            return;
        }

        upgradeLevels.Clear();

        foreach (var kvp in DataManager.Instance.queenAbilityDic)
        {
            upgradeLevels[kvp.Key] = 0;
        }
    }

    /// <summary>
    /// 능력 업그레이드 시도
    /// 자원 조건이 충족되면 레벨을 증가시키고 UI를 갱신합니다.
    /// </summary>
    /// <param name="id">강화할 능력의 ID</param>
    public void TryUpgrade(int id)
    {
        if (!IsValidAbility(id, out var ability)) return;

        int currentLevel = upgradeLevels[id];
        if (currentLevel >= ability.maxLevel) return;

        int cost = ability.levelInfo[currentLevel].cost;
        // TODO: 자원 확인 및 차감

        upgradeLevels[id]++;
        queenAbilityPanelUI.SetPopupQueenAbilityInfo(ability, upgradeLevels[id]);
    }

    /// <summary>
    /// 능력 다운그레이드 시도
    /// </summary>
    /// <param name="id">다운그레이드할 능력의 ID</param>
    public void TryDowngrade(int id)
    {
        if (!IsValidAbility(id, out var ability)) return;

        int currentLevel = upgradeLevels[id];
        if (currentLevel <= 0) return;

        // TODO: 자원 반환 처리
        RefundCurrency(0);

        upgradeLevels[id]--;
        queenAbilityPanelUI.SetPopupQueenAbilityInfo(ability, upgradeLevels[id]);
    }

    /// <summary>
    /// 지정된 ID의 능력이 유효한지 검사하고 해당 정보를 반환합니다.
    /// </summary>
    /// <returns>유효한 경우 true, 그렇지 않으면 false</returns>
    private bool IsValidAbility(int id, out QueenAbilityInfo ability)
    {
        ability = GetAbilityById(id);
        return upgradeLevels.ContainsKey(id) && ability != null;
    }

    /// <summary>
    /// 능력 데이터 리스트에서 ID에 해당하는 능력 정보를 찾습니다.
    /// </summary>
    /// <returns>QueenAbilityInfo 객체 또는 null</returns>
    private QueenAbilityInfo GetAbilityById(int id)
    {
        DataManager.Instance.queenAbilityDic.TryGetValue(id, out var ability);
        return ability;
    }

    /// <summary>
    /// 현재 레벨 기준으로 주어진 능력의 효과 값을 반환합니다.
    /// 레벨이 0일 경우 0을 반환합니다.
    /// </summary>
    /// <param name="id">효과 값을 조회할 능력 ID</param>
    /// <returns>현재 레벨 기준 효과 값</returns>
    public int GetEffectValue(int id)
    {
        if (!upgradeLevels.TryGetValue(id, out int level) || level <= 0)
            return 0;

        var ability = GetAbilityById(id);
        return ability?.levelInfo[level - 1].eff ?? 0;
    }

    /// <summary>
    /// 특정 능력의 현재 레벨을 반환합니다.
    /// </summary>
    /// <param name="id">능력 ID</param>
    /// <returns>현재 강화 레벨</returns>
    public int GetLevel(int id)
    {
        return upgradeLevels.TryGetValue(id, out int level) ? level : 0;
    }

    /// <summary>
    /// 모든 능력의 업그레이드 레벨을 초기화하고,
    /// 소모된 재화를 반환한 뒤, UI를 갱신합니다.
    /// </summary>
    public void ResetAllAbilities()
    {
        int totalRefundCost = 0;

        foreach (var kvp in DataManager.Instance.queenAbilityDic)
        {
            var ability = kvp.Value;
            int id = ability.ID;

            int currentLevel = upgradeLevels[id];

            // 레벨이 1 이상일 경우, 누적 비용 계산
            for (int i = 0; i < currentLevel; i++)
            {
                totalRefundCost += ability.levelInfo[i].cost;
            }

            // 레벨 초기화
            upgradeLevels[id] = 0;
        }

        // UI갱신
        RefreshAllAbilityItems();

        // 재화 반환 처리
        RefundCurrency(totalRefundCost);

        Utils.Log($"모든 능력을 초기화하고 {totalRefundCost}만큼 재화를 반환받음.");
    }

    /// <summary>
    /// 권능 UI 전체 갱신
    /// </summary>
    public void RefreshAllAbilityItems()
    {
        foreach (var kvp in abilityItemDict)
        {
            int id = kvp.Key;
            var item = kvp.Value;
            item.Refresh(GetLevel(id));
        }
    }

    /// <summary>
    /// UI 스크립트를 등록하고 능력 목록 UI 아이템을 생성합니다.
    /// </summary>
    /// <param name="script">UI 패널 스크립트</param>
    public void SetUIQueenAbility(QueenAbilityPanelUI script)
    {
        queenAbilityPanelUI = script;
        CreateAbilityItems();
    }

    /// <summary>
    /// 업그레이드에 사용된 재화를 반환합니다.
    /// </summary>
    /// <param name="amount"></param>
    private void RefundCurrency(int amount)
    {
        // TODO: 실제 게임 내 재화 시스템에 따라 처리
        // GameManager.Instance.AddGold(amount);
    }


    /// <summary>
    /// 능력 목록을 순회하며 각 능력의 UI 아이템을 생성합니다.
    /// </summary>
    private void CreateAbilityItems()
    {
        abilityItemDict.Clear();

        foreach (var kvp in DataManager.Instance.queenAbilityDic)
        {
            var ability = kvp.Value;

            var item = Instantiate(abilityItemPrefab, QueenAbilityPanelUI.ContentTransform);
            item.Initialize(ability, GetLevel(ability.ID));
            abilityItemDict[ability.ID] = item;
        }
    }

    /// <summary>
    /// 현재 강화 상태를 저장 데이터 형태로 변환합니다.
    /// </summary>
    /// <returns>강화 정보가 담긴 QueenAbilityUpgradeData 객체</returns>
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
    /// 저장된 강화 데이터를 적용하고 UI를 갱신합니다.
    /// </summary>
    /// <param name="data">적용할 강화 데이터</param>
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
                // 유효한 범위인지 체크
                var ability = GetAbilityById(info.id);
                if (ability != null && info.level <= ability.maxLevel)
                {
                    upgradeLevels[info.id] = info.level;
                }
            }
        }

        // UI 갱신
        RefreshAllAbilityItems();
    }

}
