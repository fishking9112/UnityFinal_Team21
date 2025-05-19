using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class GameResultUnitData
{
    public int spawnCount;
    public float allDamage;
}

public class GameResultUI : SingleUI
{
    [Header("UI Components")]
    public GameObject resultWindow;
    public GameObject dpsPopupUI;
    public Transform unitListParent;
    public GameUnitResultUI gameUnitResultUIPrefab;
    public Image mvpImg;

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

    private void Start()
    {
        titleMenuBtn.onClick.AddListener(ReturnToTitle);
        dpsPopupBtn.onClick.AddListener(() => DpsPopup(true));
        closePopupBtn.onClick.AddListener(() => DpsPopup(false));
    }

    private void OnEnable()
    {
        InitQueenResult();
        InitMiddlePanel();
        InitUnitResult();
        ApplyStageGold();
        SetMonsterMVP();
        QueenAbilityUpgradeManager.Instance.ResetQueenAbilityMonsterValues();
    }
    private void InitQueenResult()
    {
        queenLevelText.text = "Lv. " + GameManager.Instance.queen.condition.Level.Value.ToString();
    }

    private void InitMiddlePanel()
    {
        gameTimeText.text = Utils.GetMMSSTime((int)(GameManager.Instance.gameLimitTime - GameManager.Instance.curTime.Value));
        killCountText.text = "000";
        resourceText.text = GameManager.Instance.queen.condition.Gold.Value.ToString();
    }

    private void InitUnitResult()
    {
        foreach (var data in StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().gameResultUI.resultDatas)
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
}
