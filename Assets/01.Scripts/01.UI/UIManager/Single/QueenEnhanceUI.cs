using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QueenEnhanceUI : SingleUI
{
    [SerializeField] private QueenEnhanceStatusUI queenEnhanceStatusUI;
    public QueenEnhanceStatusUI QueenEnhanceStatusUI => queenEnhanceStatusUI;
    [SerializeField] private SelectInhanceItem[] itemSlots;
    [SerializeField] private QueenSkillSwapItem[] skillSwapSlots;
    [SerializeField] private GameObject queenSkillSwapPopup;
    [SerializeField] private Button skillSwapPopupExitBtn;
    [SerializeField] private Button enhanceRerollBtn;
    [SerializeField] private TextMeshProUGUI rerollCostText;

    private Dictionary<int, int> acquiredEnhanceLevels = new Dictionary<int, int>();
    public IReadOnlyDictionary<int, int> AcquiredEnhanceLevels => acquiredEnhanceLevels;

    private QueenEnhanceInfo tmpQueenEnhanceInfo;
    [HideInInspector] public bool isOpen = false;

    private const int initRerollCost = 1000;
    private const int increaseRerollCost = 100;
    private int useRerollCount = 0;

    private void Awake()
    {
        skillSwapPopupExitBtn.onClick.AddListener(CloseSkillSwapPopupUI);
        enhanceRerollBtn.onClick.AddListener(OnClickEnhanceReroll);
    }

    private void OnEnable()
    {
        skillSwapPopupExitBtn.onClick.RemoveAllListeners();
        skillSwapPopupExitBtn.onClick.AddListener(CloseSkillSwapPopupUI);

        queenSkillSwapPopup.SetActive(false);
        var randomOptions = GetRandomInhanceOptions();
        ShowSelectUI(randomOptions);
        UpdateRerollCostText();
        ClickDelay();
        isOpen = true;
    }
    private void OnDisable()
    {
        isOpen = false;
    }
    /// <summary>
    /// 리롤 버튼 클릭 시 호출
    /// </summary>
    private void OnClickEnhanceReroll()
    {
        if (GameManager.Instance.queen.condition.Gold.Value >= initRerollCost + (increaseRerollCost * useRerollCount))
        {
            // 골드 차감
            GameManager.Instance.queen.condition.Gold.Value -= initRerollCost + (increaseRerollCost * useRerollCount);
            useRerollCount++;

            // 새 옵션 가져오기 및 UI 갱신
            var randomOptions = GetRandomInhanceOptions();
            ShowSelectUI(randomOptions);
            UpdateRerollCostText();
            ClickDelay();
        }
        else
        {
            UIManager.Instance.ShowPopup("알림", "골드가 부족합니다.", () => { Utils.Log("확인."); });
        }
    }

    /// <summary>
    /// 리롤시 리롤 버튼 텍스트 변경
    /// </summary>
    private void UpdateRerollCostText()
    {
        rerollCostText.text = (initRerollCost + (increaseRerollCost * useRerollCount)).ToString();
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
    /// enhance 팝업창이 뜨자마자 선택되는 것을 방지
    /// </summary>
    public async void ClickDelay()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].isSelectable = false;
        }

        await UniTask.Delay(1000, ignoreTimeScale: true, cancellationToken: this.GetCancellationTokenOnDestroy());

        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].isSelectable = true;
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

        float value = info.state_Base + (info.state_LevelUp * (level - 1));

        switch (info.type)
        {
            case QueenEnhanceType.AddSkill:
                result = AcquireQueenSkill(info);
                break;

            case QueenEnhanceType.QueenPassive:
                ApplyQueenPassive(id, value);
                break;

            case QueenEnhanceType.MonsterPassive:
                ApplyMonsterPassive(info.brood, info.name, value, info.valueType);
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
        float amount = 0;

        switch (id)
        {
            case (int)IDQueenEnhance.QUEEN_MANA_GAUGE_RECOVERY_SPEED_UP: // 마나 회복 속도 증가
                amount = queenCondition.QueenActiveSkillGaugeRecoverySpeed * value;
                queenCondition.AdjustQueenActiveSkillGaugeRecoverySpeed(amount);
                break;

            case (int)IDQueenEnhance.QUEEN_SUMMON_GAUGE_RECOVERY_SPEED_UP: // 소환 게이지 회복 속도 증가
                amount = queenCondition.SummonGaugeRecoverySpeed * value;
                queenCondition.AdjustSummonGaugeRecoverySpeed(amount);
                break;

            case (int)IDQueenEnhance.CASTLE_HEALTH_RECOVERY_SPEED_UP: // 성벽 체력 회복량 증가
                amount = castleCondition.CastleHealthRecoverySpeed * value;
                castleCondition.AdjustCastleHealthRecoverySpeed(amount);
                break;

            case (int)IDQueenEnhance.CASTLE_MAX_HEALTH_UP: // 성벽 최대 체력 증가
                amount = castleCondition.MaxCastleHealth.Value * value;
                castleCondition.AdjustMaxHealth(amount);
                break;

            case (int)IDQueenEnhance.QUEEN_MAX_MANA_GAUGE_UP: // 여왕 마나 최대량 증가 
                amount = queenCondition.MaxQueenActiveSkillGauge.Value * value;
                queenCondition.AdjustMaxQueenActiveSkillGauge(amount);
                break;

            case (int)IDQueenEnhance.QUEEN_MAX_SUMMON_GAUGE_UP: // 여왕 소환 게이지 최대량 증가
                amount = queenCondition.MaxSummonGauge.Value * value;
                queenCondition.AdjustMaxSummonGauge(amount);
                break;
        }
    }

    /// <summary>
    /// 몬스터 강화 패시브 적용
    /// </summary>
    private void ApplyMonsterPassive(MonsterBrood brood, string name, float value, ValueType type)
    {
        foreach (var monster in MonsterManager.Instance.monsterInfoList.Values)
        {
            if (monster.monsterBrood != brood)
                continue;

            float amount = 0f;
            
            switch (type)
            {
                case ValueType.Hp:
                    float originHealth= DataManager.Instance.monsterDic[monster.id].health;
                    amount = originHealth * value;
                    monster.health += amount;
                    foreach (var controller in MonsterManager.Instance.idByMonsters[monster.id])
                        controller.UpgradeHealth(amount);
                    break;

                case ValueType.Attack:
                    float originAtt = DataManager.Instance.monsterDic[monster.id].attack;
                    amount = originAtt * value;
                    monster.attack += amount;
                    foreach (var controller in MonsterManager.Instance.idByMonsters[monster.id])
                        controller.UpgradeAttack(amount);
                    break;

                case ValueType.MoveSpeed: // 수정: AttackSpeed가 아니라 MoveSpeed
                    float originSpeed = DataManager.Instance.monsterDic[monster.id].moveSpeed;
                    amount = originSpeed * value;
                    monster.moveSpeed += amount;
                    foreach (var controller in MonsterManager.Instance.idByMonsters[monster.id])
                        controller.UpgradeMoveSpeed(amount);
                    break;

                case ValueType.AttackSpeed:
                    float originAttspd = DataManager.Instance.monsterDic[monster.id].attackSpeed;
                    amount = originAttspd * value;
                    monster.attackSpeed += amount;
                    foreach (var controller in MonsterManager.Instance.idByMonsters[monster.id])
                        controller.UpgradeAttackSpeed(amount);
                    break;
            }
        }
    }

    /// <summary>
    /// 강화할 수 있는 옵션을 무작위로 3개 뽑는다.
    /// </summary>
    private List<QueenEnhanceInfo> GetRandomInhanceOptions()
    {
        List<QueenEnhanceInfo> addSkillList = new List<QueenEnhanceInfo>();
        List<QueenEnhanceInfo> otherList = new List<QueenEnhanceInfo>();

        foreach (var pair in DataManager.Instance.queenEnhanceDic)
        {
            int id = pair.Key;
            QueenEnhanceInfo info = pair.Value;

            acquiredEnhanceLevels.TryGetValue(id, out int currentLevel);

            if (currentLevel < info.maxLevel)
            {
                if (info.type == QueenEnhanceType.AddSkill)
                {
                    addSkillList.Add(info);
                }
                else
                {
                    otherList.Add(info);
                }
            }
        }

        List<QueenEnhanceInfo> result = new List<QueenEnhanceInfo>();

        if (addSkillList.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, addSkillList.Count);
            result.Add(addSkillList[index]);
            otherList.RemoveAt(index);
        }

        while (result.Count < 3 && otherList.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, otherList.Count);
            result.Add(otherList[index]);
            otherList.RemoveAt(index);
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
    private bool AcquireQueenSkill(QueenEnhanceInfo enhanceInfo)
    {
        tmpQueenEnhanceInfo = enhanceInfo;

        if (QueenActiveSkillManager.Instance.HasAvailableSkillSlot()) // 신규 스킬 습득 가능함.
        {
            QueenActiveSkillManager.Instance.AddSkill(tmpQueenEnhanceInfo.skill_ID);

            return true;
        }
        else // 스킬이 이미 가득참.
        {
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

        QueenActiveSkillManager.Instance.AddSkill(index, tmpQueenEnhanceInfo.skill_ID);

        queenSkillSwapPopup.SetActive(false);

        GameManager.Instance.queen.condition.EnhancePoint--;
        CloseUI();
    }

    // 현재 강화 수치 레벨 다운(스킬 전용)
    private void SetMiusAcquiredEnhanceLevels(int enhanceID)
    {
        int tmp = 0;

        foreach (var item in DataManager.Instance.queenEnhanceDic)
        {
            if (item.Value.skill_ID == enhanceID)
            {
                tmp = item.Value.ID;
            }
        }

        if (acquiredEnhanceLevels.ContainsKey(tmp))
        {
            acquiredEnhanceLevels[tmp]--;

            return;
        }
    }
}
