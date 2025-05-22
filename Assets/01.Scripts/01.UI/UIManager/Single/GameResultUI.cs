using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameResultUnitData
{
    public int spawnCount;
    public float allDamage;
}

public class GameResultUI : SingleUI
{
    public bool isClear;

    [Header("UI Components")]
    public GameObject resultWindow;
    public GameObject dpsPopupUI;
    public Transform unitListParent;
    public GameUnitResultUI gameUnitResultUIPrefab;
    public Image titleImg;
    public Image mvpImg;
    public Image queenImg;
    public Image resultQueenImage;
    public Transform parentEnhanceGrid;
    public Transform parentSkillGrid;

    [Header("DescriptionPopupUI")]
    public Transform descriptionPopupUI;
    public GameObject DescriptionPopupUI => descriptionPopupUI.gameObject;
    public Image popupUIAbilityImage;
    public TextMeshProUGUI popupUIAbilityName;
    public TextMeshProUGUI popupUIAbilityDec;
    public TextMeshProUGUI popupUIAbilityLevel;

    [Header("Enhance/Skill Item Prefabs")]
    public OwnedEnhanceItem prefabsOwnedEnhanceItem;

    [Header("Text Components")]
    public TextMeshProUGUI queenLevelText;
    public TextMeshProUGUI gameTimeText;
    public TextMeshProUGUI killCountText;
    public TextMeshProUGUI resourceText;

    [Header("Button Components")]
    public Button titleMenuBtn;
    public Button dpsPopupBtn;
    public Button closePopupBtn;

    public Dictionary<int, GameResultUnitData> resultDatas = new();

    private const string backgroundVictory = "ResultBackground_Victory";
    private const string backgroundDefeat = "ResultBackground_Defeat";
    private const string resultTitleVictory = "ResultTitleVictory";
    private const string resultTitleDefeat = "ResultTitleDefeat";

    private void Start()
    {
        titleMenuBtn.onClick.AddListener(ReturnToTitle);
        dpsPopupBtn.onClick.AddListener(() => DpsPopup(true));
        closePopupBtn.onClick.AddListener(() => DpsPopup(false));
    }

    private void OnEnable()
    {
        InitResultTextImage();
        InitQueenEnhance();
        InitQueenSkill();
        InitQueenResult();
        InitMiddlePanel();
        InitUnitResult();
        ApplyStageGold();
        SetMonsterMVP();

        QueenAbilityUpgradeManager.Instance.ResetQueenAbilityMonsterValues();

        UGSManager.Instance.SaveLoad.SaveAsync().Forget();
    }

    /// <summary>
    /// 팝업 UI가 마우스를 따라다니도록 위치를 계속 갱신합니다.
    /// </summary>
    public async UniTaskVoid FollowMouse(CancellationToken token)
    {
        while (DescriptionPopupUI.activeSelf)
        {
            descriptionPopupUI.position = Input.mousePosition;
            await UniTask.Yield();
        }
    }

    private void InitResultTextImage()
    {
        var atlas = DataManager.Instance.iconAtlas;
        titleImg.sprite = atlas.GetSprite(isClear ? resultTitleVictory : resultTitleDefeat);
        resultQueenImage.sprite = atlas.GetSprite(isClear ? backgroundVictory : backgroundDefeat);
    }

    private void InitQueenEnhance()
    {
        foreach (Transform child in parentEnhanceGrid)
            Destroy(child.gameObject);

        var enhanceLevels = StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().queenEnhanceUI.AcquiredEnhanceLevels;

        foreach (var item in enhanceLevels)
        {
            var info = DataManager.Instance.queenEnhanceDic[item.Key];
            if (info.type is QueenEnhanceType.QueenPassive or QueenEnhanceType.MonsterPassive)
            {
                var enhanceItem = Instantiate(prefabsOwnedEnhanceItem, parentEnhanceGrid);
                enhanceItem.SetEnhanceItem(item.Key, true);
            }
        }
    }

    private void InitQueenSkill()
    {
        foreach (Transform child in parentSkillGrid)
            Destroy(child.gameObject);

        var enhanceLevels = StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().queenEnhanceUI.AcquiredEnhanceLevels;

        foreach (var item in enhanceLevels)
        {
            if (DataManager.Instance.queenEnhanceDic[item.Key].type == QueenEnhanceType.AddSkill)
            {
                var skillItem = Instantiate(prefabsOwnedEnhanceItem, parentSkillGrid);
                skillItem.SetEnhanceItem(item.Key, true);
            }
        }
    }

    private void InitQueenResult()
    {
        int queenId = GameManager.Instance.QueenCharaterID;
        var queenInfo = DataManager.Instance.queenStatusDic[queenId];
        queenImg.sprite = DataManager.Instance.iconAtlas.GetSprite(queenInfo.Icon);
        queenLevelText.text = $"Lv. {GameManager.Instance.queen.condition.Level.Value}";
    }

    private void InitMiddlePanel()
    {
        gameTimeText.text = Utils.GetMMSSTime((int)(GameManager.Instance.gameLimitTime - GameManager.Instance.curTime.Value));
        killCountText.text = Utils.GetThousandCommaText(GameManager.Instance.queen.condition.KillCnt.Value);
        resourceText.text = Utils.GetThousandCommaText((int)GameManager.Instance.queen.condition.Gold.Value);
    }

    private void InitUnitResult()
    {
        foreach (var data in resultDatas)
        {
            GameUnitResultUI unitResultPanel = Instantiate(gameUnitResultUIPrefab, unitListParent);
            string unitName = DataManager.Instance.monsterDic[data.Key].name;
            string unitIcon = DataManager.Instance.monsterDic[data.Key].icon;
            unitResultPanel.Init(unitIcon, unitName, data.Value.spawnCount, data.Value.allDamage);
        }
    }

    private void ApplyStageGold()
    {
        int goldToAdd = Mathf.FloorToInt(GameManager.Instance.queen.condition.Gold.Value);
        GameManager.Instance.AddGold(goldToAdd);
    }

    private void SetMonsterMVP()
    {
        int mvpId = int.MinValue;
        float mvpPerDamage = float.MinValue;

        foreach (var data in StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().gameResultUI.resultDatas)
        {
            if (mvpPerDamage <= data.Value.allDamage / data.Value.spawnCount)
            {
                mvpPerDamage = data.Value.allDamage / data.Value.spawnCount;
                mvpId = data.Key;
            }
        }

        if (mvpId != int.MinValue)
        {
            string unitIcon = DataManager.Instance.monsterDic[mvpId].icon;
            mvpImg.sprite = DataManager.Instance.iconAtlas.GetSprite(unitIcon);
        }
        else
        {
            mvpImg.gameObject.SetActive(false);
        }
    }

    private void ReturnToTitle()
    {
        GameManager.Instance.curTime.Value = 0f;
        SceneLoadManager.Instance.LoadScene(LoadSceneEnum.MenuScene).Forget();
    }

    private void DpsPopup(bool value)
    {
        dpsPopupUI.SetActive(value);
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

        float previewValue = (currentLevel / 2f) * (2 * info.state_Base + (currentLevel - 1) * info.state_LevelUp);

        string formattedValue = QueenEnhanceStatusUI.PercentValueTypes.Contains(info.valueType)
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
