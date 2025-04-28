using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class QueenEnhanceUI : SingleUI
{
    [SerializeField] private QueenEnhanceStatusUI queenEnhanceStatusUI;
    [SerializeField] private SelectInhanceItem[] itemSlots;

    private Dictionary<int, int> acquiredEnhanceLevels = new Dictionary<int, int>();
    public IReadOnlyDictionary<int, int> AcquiredEnhanceLevels => acquiredEnhanceLevels;

    private void OnEnable()
    {
        var randomOptions = GetRandomInhanceOptions();
        ShowSelectUI(randomOptions);
    }

    /// <summary>
    /// 강화 선택 UI를 표시하고 슬롯에 정보를 채워 넣는다.
    /// </summary>
    public void ShowSelectUI(List<QueenEnhanceInfo> list)
    {
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().ShowWindow<QueenEnhanceUI>();
        queenEnhanceStatusUI.RefreshStatus();

        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].SetInfo(list[i]);
        }
    }

    /// <summary>
    /// 강화 선택창 UI를 닫는다.
    /// </summary>
    public void CloseUI()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].ResetButton();
        }
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().HideWindow();
    }

    /// <summary>
    /// 강화 항목을 실제로 적용한다.
    /// </summary>
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
    /// 여왕 강화 패시브 적용
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
    /// 몬스터 강화 패시브 적용
    /// </summary>
    private void ApplyMonsterPassive(MonsterBrood brood, string name, int value)
    {
        foreach (var monster in MonsterManager.Instance.monsterInfoList.Values)
        {
            if (monster.monsterBrood != brood)
                continue;

            if (name.Contains("체력"))
            {
                MonsterManager.Instance.monsterInfoList[monster.id].health += value;
                foreach (var monsterController in MonsterManager.Instance.idByMonsters[monster.id])
                {
                    monsterController.UpgradeHealth(value);
                }
            }
            else if (name.Contains("공격력"))
            {
                MonsterManager.Instance.monsterInfoList[monster.id].attack += value;
                foreach (var monsterController in MonsterManager.Instance.idByMonsters[monster.id])
                {
                    monsterController.UpgradeAttack(value);
                }
            }
            else if (name.Contains("이동속도"))
            {
                MonsterManager.Instance.monsterInfoList[monster.id].moveSpeed += value;
                foreach (var monsterController in MonsterManager.Instance.idByMonsters[monster.id])
                {
                    monsterController.UpgradeMoveSpeed(value);
                }
            }
        }
    }

    /// <summary>
    /// 강화할 수 있는 옵션을 무작위로 3개 뽑는다.
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
    /// 특정 강화 ID의 현재 강화 수치 총합을 계산
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
    /// 특정 강화 ID의 현재 강화 레벨을 반환
    /// </summary>
    public int GetEnhanceLevel(int id)
    {
        return acquiredEnhanceLevels.TryGetValue(id, out var level) ? level : 0;
    }
}
