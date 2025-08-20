using Cysharp.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class QueenEnhanceStatusUI : MonoBehaviour
{
    [Header("UI Component")]
    [SerializeField] private QueenCondition queenCondition;
    [SerializeField] private Transform descriptionPopupUI;
    public GameObject DescriptionPopupUI => descriptionPopupUI.gameObject;
    [SerializeField] private LocalizeStringEvent statusManaLocalize;
    [SerializeField] private LocalizeStringEvent statusManaRegenLocalize;
    [SerializeField] private LocalizeStringEvent statusSummonLocalize;
    [SerializeField] private LocalizeStringEvent statusSummonRegenLocalize;
    [SerializeField] private LocalizeStringEvent statusCastleHpLocalize;
    [SerializeField] private LocalizeStringEvent statusCastleHpRegenLocalize;

    [Header("DescriptionPopupUI")]
    [SerializeField] private Image popupUIAbilityImage;
    [SerializeField] private TextMeshProUGUI popupUIAbilityName;
    [SerializeField] private LocalizeStringEvent abilityNameLocalize;
    [SerializeField] private TextMeshProUGUI popupUIAbilityDec;
    [SerializeField] private TextMeshProUGUI popupUIAbilityLevel;

    [Header("EnhanceGrid")]
    [SerializeField] private Transform enhanceContent;
    [SerializeField] private OwnedEnhanceItem prefabsOwnedEnhanceItem;

    // 앞으로 추가될 퍼센트 타입들도 여기 넣으면 됨
    //public static readonly HashSet<ValueType> PercentValueTypes = new HashSet<ValueType>
    //{
    //    ValueType.MoveSpeed,
    //};

    /// <summary>
    /// 팝업 UI가 마우스를 따라다니도록 위치를 계속 갱신합니다.
    /// </summary>
    /// 
    public async UniTaskVoid FollowMouse(CancellationToken token)
    {
        while (descriptionPopupUI != null
               && descriptionPopupUI.gameObject.activeInHierarchy)
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

        if(GameManager.Instance.QueenCharaterID == 0)
            GameManager.Instance.QueenCharaterID = DataManager.Instance.queenStatusDic.First().Key;

        var statusBuilder = new StringBuilder();

        // 마나, 게이지, 체력 상태
        AppendManaStatus(statusBuilder);
        AppendManaRegenStatus(statusBuilder);
        AppendSummonGaugeStatus(statusBuilder);
        AppendSummonRegenStatus(statusBuilder);
        AppendCastleHpStatus(statusBuilder);
        AppendCastleHpRegenStatus(statusBuilder);

        // 텍스트 UI에 각각 설정
        // statusText.text = statusBuilder.ToString();

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


        foreach (var items in queenEnhanceUI.AcquiredEnhanceLevels)
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
        // builder.AppendLine($"마나 : {(int)curMana} / {(int)maxMana}");
        StringManager.Instance.SetString("9902402", statusManaLocalize, ((int)curMana).ToString(), ((int)maxMana).ToString());

    }

    /// <summary>
    /// 마나 상태를 문자열로 추가합니다.
    /// </summary>
    private void AppendManaRegenStatus(StringBuilder builder)
    {
        // 마나 회복량 = 기본 회복량 + 강화 효과
        float manaRegenBase = DataManager.Instance.queenStatusDic[GameManager.Instance.QueenCharaterID].mana_Recorvery + GameManager.Instance.queen.condition.AbilityUpgrade_QueenActiveSkillGaugeRecoverySpeed;
        float manaRegenEnhance = queenCondition.QueenActiveSkillGaugeRecoverySpeed - manaRegenBase;
        // builder.AppendLine($"마나 회복량 : {FormatNumber(manaRegenBase)} + {FormatNumber(manaRegenEnhance)} / s");
        StringManager.Instance.SetString("9902403", statusManaRegenLocalize, FormatNumber(manaRegenBase).ToString(), FormatNumber(manaRegenEnhance).ToString());
    }

    /// <summary>
    /// 소환 게이지 상태를 문자열로 추가합니다.
    /// </summary>
    private void AppendSummonGaugeStatus(StringBuilder builder)
    {
        float curSummongauge = queenCondition.CurSummonGauge.Value;
        float maxSummonGauge = queenCondition.MaxSummonGauge.Value;
        // builder.AppendLine($"소환 게이지 : {FormatNumber(curSummongauge)} / {FormatNumber(maxSummonGauge)}");
        StringManager.Instance.SetString("9902404", statusSummonLocalize, FormatNumber(curSummongauge).ToString(), FormatNumber(maxSummonGauge).ToString());
    }

    /// <summary>
    /// 소환 회복량 상태를 문자열로 추가합니다.
    /// </summary>
    private void AppendSummonRegenStatus(StringBuilder builder)
    {
        float summonRegenBase = DataManager.Instance.queenStatusDic[GameManager.Instance.QueenCharaterID].summon_Recorvery + GameManager.Instance.queen.condition.AbilityUpgrade_SummonGaugeRecoverySpeed;
        float summonRegenEnhance = queenCondition.SummonGaugeRecoverySpeed - summonRegenBase;
        // builder.AppendLine($"소환 회복량 : {FormatNumber(summonRegenBase)} + {FormatNumber(summonRegenEnhance)} / s");
        StringManager.Instance.SetString("9902405", statusSummonRegenLocalize, FormatNumber(summonRegenBase).ToString(), FormatNumber(summonRegenEnhance).ToString());
    }

    /// <summary>
    /// 캐슬 체력 상태를 문자열로 추가합니다.
    /// </summary>
    private void AppendCastleHpStatus(StringBuilder builder)
    {
        float curCastleHp = GameManager.Instance.castle.condition.CurCastleHealth.Value;
        float maxCastleHp = GameManager.Instance.castle.condition.MaxCastleHealth.Value;
        // builder.AppendLine($"캐슬 체력 : {FormatNumber(curCastleHp)} / {FormatNumber(maxCastleHp)}");
        StringManager.Instance.SetString("9902406", statusCastleHpLocalize, FormatNumber(curCastleHp).ToString(), FormatNumber(maxCastleHp).ToString());
    }

    /// <summary>
    /// 캐슬 체력 회복량 상태를 문자열로 추가합니다.
    /// </summary>
    private void AppendCastleHpRegenStatus(StringBuilder builder)
    {
        float castleHpRegenBase = GameManager.Instance.castle.condition.initCastleHealthRecoverySpeed + GameManager.Instance.castle.condition.AbilityUpgrade_CastleHealthRecoverySpeed;
        float castleHpRegenEnhance = GameManager.Instance.castle.condition.CastleHealthRecoverySpeed - castleHpRegenBase;
        // builder.AppendLine($"캐슬 회복량 : {FormatNumber(castleHpRegenBase)} + {FormatNumber(castleHpRegenEnhance)} / s");
        StringManager.Instance.SetString("9902407", statusCastleHpRegenLocalize, FormatNumber(castleHpRegenBase).ToString(), FormatNumber(castleHpRegenEnhance).ToString());
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
        // popupUIAbilityName.text = info.name;
        StringManager.Instance.SetString(info.name, abilityNameLocalize);

        float previewValue = (currentLevel / 2f) * (2 * info.state_Base + (currentLevel - 1) * info.state_LevelUp);

        string formattedValue = $"{previewValue * 100:F0}%";

        SetAbilityDecText(info, formattedValue).Forget();

        if (info.type != QueenEnhanceType.AddSkill)
        {
            popupUIAbilityLevel.text = "Lv. " + StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().queenEnhanceUI.AcquiredEnhanceLevels[enhanceID].ToString();
        }
        else
        {
            popupUIAbilityLevel.text = "-";
        }
    }

    public async UniTask SetAbilityDecText(QueenEnhanceInfo info, string formattedValue)
    {
        var description = await StringManager.Instance.GetString(info.description);
        popupUIAbilityDec.text = string.Format(description, formattedValue);
    }
}
