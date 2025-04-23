using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(QueenEnhanceUI))]
public class QueenEnhanceController : MonoBehaviour
{
    private Dictionary<int, int> acquiredEnhanceLevels = new Dictionary<int, int>();
    public IReadOnlyDictionary<int, int> AcquiredEnhanceLevels => acquiredEnhanceLevels;

    public QueenEnhanceUI queenEnhanceUI;

    /// <summary>
    /// 창이 뜨면 매번 랜덤으로 호출됨
    /// </summary>
    private void OnEnable()
    {
        var randomOptions = GetRandomInhanceOptions();
        queenEnhanceUI.ShowSelectUI(randomOptions);
    }

    /// <summary>
    /// 현재 강화 목록에서 레벨이 MaxLevel이 아닌 3개 항목을 무작위로 선택
    /// </summary>
    private List<QueenEnhanceInfo> GetRandomInhanceOptions()
    {
        List<QueenEnhanceInfo> availableList = new List<QueenEnhanceInfo>();

        foreach (var pair in DataManager.Instance.queenEnhanceDic)
        {
            int id = pair.Key;
            QueenEnhanceInfo info = pair.Value;

            acquiredEnhanceLevels.TryGetValue(id, out int currentLevel);

            if (currentLevel < info.maxLevel)
                availableList.Add(info);
        }

        List<QueenEnhanceInfo> result = new List<QueenEnhanceInfo>();

        while (result.Count < 3 && availableList.Count > 0)
        {
            int index = Random.Range(0, availableList.Count);
            result.Add(availableList[index]);
            availableList.RemoveAt(index);
        }

        return result;
    }

    /// <summary>
    /// 강화 항목 선택 시 실제 적용되는 함수
    /// </summary>
    /// <param name="info">강화 항목 정보</param>
    public void ApplyInhance(QueenEnhanceInfo info)
    {
        int id = info.ID;

        if (acquiredEnhanceLevels.ContainsKey(id))
            acquiredEnhanceLevels[id]++;
        else
            acquiredEnhanceLevels[id] = 1;

        int level = acquiredEnhanceLevels[id];

        Utils.Log($"{info.name} 강화 적용, 현재 레벨: {level}");

        int value = info.state_Base + info.state_LevelUp * (level - 1);

        switch (info.type)
        {
            case QueenEnhanceType.QueenPassive:
                ApplyQueenPassive(id, value);
                break;

            case QueenEnhanceType.MonsterPassive:
                ApplyMonsterPassive(info.brood, info.name, value);
                break;

            case QueenEnhanceType.Point:
                GameManager.Instance.queen.condition.AdjustEvolutionPoint(1f);
                break;
        }
    }

    /// <summary>
    /// 여왕 강화 스탯 적용 처리
    /// </summary>
    private void ApplyQueenPassive(int id, int value)
    {
        var condition = GameManager.Instance.queen.condition;

        switch (id)
        {
            case 1002: // 마나 회복 속도 증가
                condition.AdjustQueenActiveSkillGaugeRecoverySpeed(value);
                break;

            case 1003: // 소환 게이지 회복 속도 증가
                condition.AdjustSummonGaugeRecoverySpeed(value);
                break;
        }
    }

    /// <summary>
    /// 몬스터 강화 스탯 적용 처리 (종족 + 항목 이름 기반 분기)
    /// </summary>
    private void ApplyMonsterPassive(MonsterBrood brood, string name, int value)
    {
        foreach (var monster in MonsterManager.Instance.monsterInfoList.Values)
        {
            if (monster.monsterBrood != brood)
                continue;

            if (name.Contains("체력"))
            {
                monster.health += value;
                foreach (var monsterController in MonsterManager.Instance.idByMonsters[monster.id])
                {
                    monsterController.HealthStatUpdate();
                }
            }
            else if (name.Contains("공격력"))
            {
                monster.attack += value;
            }
            else if (name.Contains("이동속도"))
            {
                monster.moveSpeed += value;
            }
        }
    }

    /// <summary>
    /// 강화 수치 총합을 반환 (현재 레벨까지 누적 계산)
    /// </summary>
    public int GetEnhanceValueByID(int id)
    {
        if (!acquiredEnhanceLevels.TryGetValue(id, out int level) || level <= 0)
            return 0;

        if (!DataManager.Instance.queenEnhanceDic.TryGetValue(id, out var info))
            return 0;

        int total = 0;
        for (int i = 1; i <= level; i++)
        {
            total += info.state_Base + info.state_LevelUp * (i - 1);
        }

        return total;
    }

    /// <summary>
    /// 해당 ID의 현재 강화 레벨 반환
    /// </summary>
    public int GetEnhanceLevel(int id)
    {
        return acquiredEnhanceLevels.TryGetValue(id, out var level) ? level : 0;
    }
}
