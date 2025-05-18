using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QueenEnhanceUI : SingleUI
{
    [SerializeField] private QueenEnhanceStatusUI queenEnhanceStatusUI;
    [SerializeField] private SelectInhanceItem[] itemSlots;
    [SerializeField] private QueenSkillSwapItem[] skillSwapSlots;
    [SerializeField] private GameObject queenSkillSwapPopup;
    [SerializeField] private Button SkillSwapPopupExitBtn;

    private Dictionary<int, int> acquiredEnhanceLevels = new Dictionary<int, int>();
    public IReadOnlyDictionary<int, int> AcquiredEnhanceLevels => acquiredEnhanceLevels;

    private int tmpSkillID;

    private void Awake()
    {
        SkillSwapPopupExitBtn.onClick.AddListener(CloseSkillSwapPopupUI);
    }

    private void OnEnable()
    {
        SkillSwapPopupExitBtn.onClick.RemoveAllListeners();
        SkillSwapPopupExitBtn.onClick.AddListener(CloseSkillSwapPopupUI);

        queenSkillSwapPopup.SetActive(false);
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
    /// 스킬 교체 팝업 UI 닫기
    /// </summary>
    private void CloseSkillSwapPopupUI()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].ResetButton();
        }
        queenSkillSwapPopup.SetActive(false);
    }

    /// <summary>
    /// 강화 항목을 실제로 적용한다.
    /// </summary>
    public bool ApplyInhance(QueenEnhanceInfo info)
    {
        bool result = true;

        int id = info.ID;

        if (acquiredEnhanceLevels.ContainsKey(id))
            acquiredEnhanceLevels[id]++;
        else
            acquiredEnhanceLevels[id] = 1;

        int level = acquiredEnhanceLevels[id];

        Utils.Log($"{info.name} 강화 적용, 현재 레벨: {level}");

        float value = info.state_Base + info.state_LevelUp * (level - 1);

        switch (info.type)
        {
            case QueenEnhanceType.AddSkill:
                result = AcquireQueenSkill(info.skill_ID);
                break;

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

        return result;
    }

    /// <summary>
    /// 여왕 강화 패시브 적용
    /// </summary>
    private void ApplyQueenPassive(int id, float value)
    {
        var queenCondition = GameManager.Instance.queen.condition;
        var castleCondition = GameManager.Instance.castle.condition;

        switch (id)
        {
            case (int)IDQueenEnhance.QUEEN_MANA_GAUGE_RECOVERY_SPEED_UP: // 마나 회복 속도 증가
                queenCondition.AdjustQueenActiveSkillGaugeRecoverySpeed(value);
                break;

            case (int)IDQueenEnhance.QUEEN_SUMMON_GAUGE_RECOVERY_SPEED_UP: // 소환 게이지 회복 속도 증가
                queenCondition.AdjustSummonGaugeRecoverySpeed(value);
                break;

            case (int)IDQueenEnhance.CASTLE_HEALTH_RECOVERY_SPEED_UP: // 성벽 체력 회복량 증가
                castleCondition.AdjustCastleHealthRecoverySpeed(value);
                break;

            case (int)IDQueenEnhance.CASTLE_MAX_HEALTH_UP: // 성벽 최대 체력 증가
                castleCondition.AdjustMaxHealth(value);
                break;

            case (int)IDQueenEnhance.QUEEN_MAX_MANA_GAUGE_UP: // 여왕 마나 최대량 증가 

                queenCondition.AdjustMaxQueenActiveSkillGauge(value);
                break;

            case (int)IDQueenEnhance.QUEEN_MAX_SUMMON_GAUGE_UP: // 여왕 소환 게이지 최대량 증가

                queenCondition.AdjustMaxSummonGauge(value);
                break;
        }
    }

    /// <summary>
    /// 몬스터 강화 패시브 적용
    /// </summary>
    private void ApplyMonsterPassive(MonsterBrood brood, string name, float value)
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
            int index = UnityEngine.Random.Range(0, availableList.Count);
            result.Add(availableList[index]);
            availableList.RemoveAt(index);
        }

        return result;
    }

    /// <summary>
    /// 특정 강화 ID의 현재 강화 수치 총합을 계산
    /// </summary>
    public float GetEnhanceValueByID(int id)
    {
        if (!acquiredEnhanceLevels.TryGetValue(id, out int level) || level <= 0)
            return 0;

        if (!DataManager.Instance.queenEnhanceDic.TryGetValue(id, out var info))
            return 0;

        float total = 0;
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

    // 스킬 획득 함수명
    private bool AcquireQueenSkill(int id)
    {
        if(QueenActiveSkillManager.Instance.HasAvailableSkillSlot()) // 신규 스킬 습득 가능함.
        {
            QueenActiveSkillManager.Instance.AddSkill(id);

            return true;
        }
        else // 스킬이 이미 가득참.
        {
            tmpSkillID = id;

            queenSkillSwapPopup.SetActive(true);

            for (int i = 0; i < skillSwapSlots.Length; i++)
            {
                skillSwapSlots[i].SetSkillinfo(QueenActiveSkillManager.Instance.ReturnSkillIDbyIndex(skillSwapSlots[i].Index));
            }

            return false;
        }
    }

    public void SwapClickEvent(int index, int skillID)
    {
        SetMiusAcquiredEnhanceLevels(skillID);

        QueenActiveSkillManager.Instance.AddSkill(index, tmpSkillID);

        queenSkillSwapPopup.SetActive(false);

        GameManager.Instance.queen.condition.EnhancePoint--;
        CloseUI();
    }

    // 현재 강화 수치 레벨 다운(스킬 전용)
    private void SetMiusAcquiredEnhanceLevels(int id)
    {
        if (acquiredEnhanceLevels.ContainsKey(id))
            acquiredEnhanceLevels[id]--;
    }
}
