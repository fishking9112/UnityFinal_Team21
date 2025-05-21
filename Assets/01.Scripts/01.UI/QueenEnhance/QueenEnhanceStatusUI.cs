using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QueenEnhanceStatusUI : MonoBehaviour
{
    [Header("UI Component")]
    [SerializeField] private QueenCondition queenCondition;
    [SerializeField] private Transform descriptionPopupUI;
    public GameObject DescriptionPopupUI => descriptionPopupUI.gameObject;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI enhanceText;

    [Header("DescriptionPopupUI")]
    [SerializeField] private Image popupUIAbilityImage;
    [SerializeField] private TextMeshProUGUI popupUIAbilityName;
    [SerializeField] private TextMeshProUGUI popupUIAbilityDec;
    [SerializeField] private TextMeshProUGUI popupUIAbilityLevel;

    [Header("EnhanceGrid")]
    [SerializeField] private Transform enhanceContent;
    [SerializeField] private OwnedEnhanceItem prefabsOwnedEnhanceItem;

    // 앞으로 추가될 퍼센트 타입들도 여기 넣으면 됨
    public static readonly HashSet<ValueType> PercentValueTypes = new HashSet<ValueType>
    {
        ValueType.MoveSpeed,
    };

    /// <summary>
    /// 팝업 UI가 마우스를 따라다니도록 위치를 계속 갱신합니다.
    /// </summary>
    public async UniTaskVoid FollowMouse()
    {
        while (DescriptionPopupUI.activeSelf)
        {
            descriptionPopupUI.position = Input.mousePosition;
            await UniTask.Yield();
        }
    }

    /// <summary>
    /// 퀸의 상태 정보를 설정합니다.
    /// </summary>
    public void SetQueenCondition(QueenCondition queenCondition)
    {
        this.queenCondition = queenCondition;
    }

    /// <summary>
    /// 퀸의 강화 상태 UI를 갱신합니다.
    /// </summary>
    public void RefreshStatus()
    {
        if (queenCondition == null)
            SetQueenCondition(GameManager.Instance.queen.condition);

        var statusBuilder = new StringBuilder();
        var enhanceBuilder = new StringBuilder();

        // 마나, 게이지, 체력 상태
        AppendManaStatus(statusBuilder);
        AppendSummonGaugeStatus(statusBuilder);
        AppendSummonRegenStatus(statusBuilder);
        AppendCastleHpStatus(statusBuilder);
        AppendCastleHpRegenStatus(statusBuilder);

        // 종족 강화 효과는 별도 builder 사용
        AppendBroodEnhanceStatus(enhanceBuilder);

        // 텍스트 UI에 각각 설정
        statusText.text = statusBuilder.ToString();
        enhanceText.text = enhanceBuilder.ToString();

        descriptionPopupUI.gameObject.SetActive(false);

        GenerateOwnedEnhanceItems();
    } 

    private void GenerateOwnedEnhanceItems()
    {
        foreach (Transform child in enhanceContent)
        {
            Destroy(child.gameObject);
        }

        QueenEnhanceUI queenEnhanceUI = StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().queenEnhanceUI;


        foreach(var items in queenEnhanceUI.AcquiredEnhanceLevels)
        {
            OwnedEnhanceItem ownedEnhanceItem = Instantiate(prefabsOwnedEnhanceItem, enhanceContent);
            ownedEnhanceItem.SetEnhanceItem(items.Key, false);
        }
    }

    /// <summary>
    /// 마나 상태를 문자열로 추가합니다.
    /// </summary>
    private void AppendManaStatus(StringBuilder builder)
    {
        float curMana = queenCondition.CurQueenActiveSkillGauge.Value;
        float maxMana = queenCondition.MaxQueenActiveSkillGauge.Value;
        builder.AppendLine($"마나 : {(int)curMana} / {(int)maxMana}");

        // 마나 회복량 = 기본 회복량 + 강화 효과
        float manaRegenBase = DataManager.Instance.queenStatusDic[GameManager.Instance.QueenCharaterID].mana_Recorvery;
        float manaRegenEnhance = queenCondition.QueenActiveSkillGaugeRecoverySpeed - manaRegenBase;
        builder.AppendLine($"마나 회복량 : {FormatNumber(manaRegenBase)} + {FormatNumber(manaRegenEnhance)} / sec");
    }

    /// <summary>
    /// 소환 게이지 상태를 문자열로 추가합니다.
    /// </summary>
    private void AppendSummonGaugeStatus(StringBuilder builder)
    {
        float curSummongauge = queenCondition.CurSummonGauge.Value;
        float maxSummonGauge = queenCondition.MaxSummonGauge.Value;
        builder.AppendLine($"소환 게이지 : {FormatNumber(curSummongauge)} / {FormatNumber(maxSummonGauge)}");
    }

    /// <summary>
    /// 소환 회복량 상태를 문자열로 추가합니다.
    /// </summary>
    private void AppendSummonRegenStatus(StringBuilder builder)
    {
        float summonRegenBase = DataManager.Instance.queenStatusDic[GameManager.Instance.QueenCharaterID].summon_Recorvery;
        float summonRegenEnhance = queenCondition.SummonGaugeRecoverySpeed - summonRegenBase;
        builder.AppendLine($"소환 회복량 : {FormatNumber(summonRegenBase)} + {FormatNumber(summonRegenEnhance)} / sec");
    }

    /// <summary>
    /// 캐슬 체력 상태를 문자열로 추가합니다.
    /// </summary>
    private void AppendCastleHpStatus(StringBuilder builder)
    {
        float curCastleHp = GameManager.Instance.castle.condition.CurCastleHealth.Value;
        float maxCastleHp = GameManager.Instance.castle.condition.MaxCastleHealth.Value;
        builder.AppendLine($"캐슬 체력 : {FormatNumber(curCastleHp)} / {FormatNumber(maxCastleHp)}");
    }

    /// <summary>
    /// 캐슬 체력 회복량 상태를 문자열로 추가합니다.
    /// </summary>
    private void AppendCastleHpRegenStatus(StringBuilder builder)
    {
        float castleHpRegenBase = GameManager.Instance.castle.condition.initCastleHealthRecoverySpeed;
        float castleHpRegenEnhance = GameManager.Instance.castle.condition.CastleHealthRecoverySpeed - castleHpRegenBase;
        builder.AppendLine($"캐슬 회복량 : {FormatNumber(castleHpRegenBase)} + {FormatNumber(castleHpRegenEnhance)} / sec");
    }

    /// <summary>
    /// 종족별 강화 효과(특히 MonsterPassive)를 문자열로 추가합니다.
    /// </summary>
    private void AppendBroodEnhanceStatus(StringBuilder builder)
    {
        var acquiredEnhances = StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().queenEnhanceUI.AcquiredEnhanceLevels;
        var orderedBroods = new List<string>();

        // 종족별 강화 항목을 추출
        foreach (var enhanceID in acquiredEnhances.Keys)
        {
            if (acquiredEnhances[enhanceID] <= 0) continue;

            var info = DataManager.Instance.queenEnhanceDic[enhanceID];
            if (info.type != QueenEnhanceType.MonsterPassive) continue;

            if (!orderedBroods.Contains(info.brood.ToString()))
            {
                orderedBroods.Add(info.brood.ToString());
            }
        }

        // 각 종족별 강화 효과 출력
        foreach (var brood in orderedBroods)
        {
            builder.AppendLine($"{brood}");

            foreach (var info in DataManager.Instance.queenEnhanceDic.Values)
            {
                if (info.brood.ToString() != brood || info.type != QueenEnhanceType.MonsterPassive) continue;

                int level = StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().queenEnhanceUI.GetEnhanceLevel(info.ID);
                if (level <= 0) continue;

                float value = 0;
                for (int i = 1; i <= level; i++)
                {
                    value += info.state_Base + info.state_LevelUp * (i - 1);
                }

                string formattedValue = PercentValueTypes.Contains(info.valueType) ? $"+{value * 100:F0}%" : $"+{value}";

                builder.AppendLine($"- {info.name} : Lv.{level} ({formattedValue})");
            }

            builder.AppendLine();
            builder.AppendLine("─────────────────");
            builder.AppendLine();
        }
    }

    /// <summary>
    /// 숫자를 보기 좋은 형식으로 변환합니다.
    /// </summary>
    private string FormatNumber(float value)
    {
        return value % 1 == 0 ? ((int)value).ToString() : value.ToString("F1");
    }

    /// <summary>
    /// 보유 현황의 마우스 오버 팝업창UI 표기
    /// </summary>
    /// <param name="enhanceID"></param>
    public void SetDescriptionPopupUIInfo(int enhanceID)
    {
        QueenEnhanceInfo info = DataManager.Instance.queenEnhanceDic[enhanceID];

        int currentLevel = StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().queenEnhanceUI.GetEnhanceLevel(info.ID);

        popupUIAbilityImage.sprite = DataManager.Instance.iconAtlas.GetSprite(info.Icon);
        popupUIAbilityName.text = info.name;

        float previewValue = currentLevel == 0
            ? info.state_Base
            : info.state_Base + (info.state_LevelUp * currentLevel);

        string formattedValue = PercentValueTypes.Contains(info.valueType)
            ? $"{previewValue * 100:F0}%"
            : $"{previewValue}";

        popupUIAbilityDec.text = info.description.Replace("n", formattedValue);

        if (info.type != QueenEnhanceType.AddSkill)
        {
            popupUIAbilityLevel.text = "Lv. " + StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().queenEnhanceUI.AcquiredEnhanceLevels[enhanceID].ToString();
        }
        else
        {
            popupUIAbilityLevel.text = "-";
        }
    }
}
