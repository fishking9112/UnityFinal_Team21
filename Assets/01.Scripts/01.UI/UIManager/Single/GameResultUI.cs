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
    public TextMeshProUGUI gameTimeText;
    public TextMeshProUGUI resourceText;
    public Transform unitListParent;
    public GameUnitResultUI gameUnitResultUIPrefab;
    public Button titleMenuBtn;

    public Dictionary<int, GameResultUnitData> resultDatas = new();

    private void Start()
    {
        titleMenuBtn.onClick.AddListener(ReturnToTitle);
    }

    private void OnEnable()
    {
        InitMiddlePanel();
        InitUnitResult();
        ApplyStageGold();
        QueenAbilityUpgradeManager.Instance.ResetQueenAbilityMonsterValues();
    }

    private void InitMiddlePanel()
    {
        // TODO : 1800f -> 게임 시간 관리 로직 리팩토링 시 수정
        gameTimeText.text = Utils.GetMMSSTime((int)(1800f - GameManager.Instance.curTime.Value));
        resourceText.text = GameManager.Instance.queen.condition.Gold.Value.ToString();
    }

    private void InitUnitResult()
    {
        foreach (var data in StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().gameResultUI.resultDatas)
        {
            GameUnitResultUI unitResultPanel = Instantiate(gameUnitResultUIPrefab, unitListParent);
            string unitName = DataManager.Instance.monsterDic[data.Key].name;
            unitResultPanel.Init(unitName, data.Value.spawnCount, data.Value.allDamage);
        }
    }

    private void ApplyStageGold()
    {
        int goldToAdd = Mathf.FloorToInt(GameManager.Instance.queen.condition.Gold.Value);
        GameManager.Instance.AddGold(goldToAdd);
    }

    private void ReturnToTitle()
    {
        SceneLoadManager.Instance.LoadScene(LoadSceneEnum.MenuScene).Forget();
    }
}
