using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QueenAbilityUpgradeInfo
{
    public int id;
    public int level;
}

public class QueenAbilityUpgradeContainer
{
    public List<QueenAbilityUpgradeInfo> upgradeList = new List<QueenAbilityUpgradeInfo>();
}


public class QueenAbilityUpgradeManager : MonoSingleton<QueenAbilityUpgradeManager>
{
    [SerializeField] private QueenAbilityData abilityData;

    private Dictionary<int, int> upgradeLevels;

    /// <summary>
    /// 업그레이드 데이터를 초기화합니다.
    /// 게임 시작 시 호출하여 능력들의 업그레이드 레벨을 0으로 세팅합니다.
    /// </summary>
    public void Initialize()
    {
        if (abilityData == null)
        {
            Utils.LogError("QueenAbilityData 참조가 없음");
            return;
        }

        upgradeLevels = new Dictionary<int, int>();

        foreach (var info in abilityData.infoList)
        {
            upgradeLevels[info.id] = 0;
        }
    }

    /// <summary>
    /// 능력 업그레이드 시도
    /// 업그레이드 성공 시 true를 반환
    /// </summary>
    /// <param name="id">강화할 능력의 ID</param>
    /// <returns>업그레이드 성공 여부</returns>
    public bool TryUpgrade(int id)
    {
        if (abilityData == null || !upgradeLevels.ContainsKey(id))
            return false;

        var ability = abilityData.infoList.Find(x => x.id == id);
        if (ability == null) return false;

        int currentLevel = upgradeLevels[id];
        if (currentLevel >= ability.maxLevel)
            return false;

        int cost = ability.levelInfo[currentLevel].cost;

        // TODO: 자원 확인 및 차감 로직
        // if ()
        // {
        //     return false
        // }

        upgradeLevels[id]++;
        return true;
    }

    /// <summary>
    /// 현재 레벨 기준으로 주어진 능력의 효과 값을 반환합니다.
    /// 레벨이 0일 경우 0을 반환합니다.
    /// </summary>
    /// <param name="id">효과 값을 조회할 능력 ID</param>
    /// <returns>현재 레벨 기준 효과 값</returns>
    public int GetEffectValue(int id)
    {
        if (abilityData == null || !upgradeLevels.ContainsKey(id))
            return 0;

        int level = upgradeLevels[id];
        return abilityData.infoList[id].levelInfo[level - 1].eff;
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
}
