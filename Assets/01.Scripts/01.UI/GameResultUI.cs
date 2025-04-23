using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class GameResultUI : MonoBehaviour
{
    public GameObject resultWindow;
    public TextMeshProUGUI gameTimeText;
    public TextMeshProUGUI resourceText;


    public Transform unitListParent;
    public GameUnitResultUI gameUnitResultUI;
    public Button titleMenuBtn;


    public void InitMiddlePanel()
    {
        gameTimeText.text = Utils.GetMMSSTime((int)(1800f - InGameUIManager.Instance.inGameHUD.GetTimer())); // TODO : 1800f 수정 필요 (UI 리펙토링 하면서 수정)
        resourceText.text = GameManager.Instance.queen.condition.Gold.Value.ToString();// + GameManager.Instance.GetGold();
    }

    public void InitUnitResult()
    {
        foreach (var data in InGameUIManager.Instance.gameResult.resultDatas)
        {
            GameUnitResultUI unitResultPanel = Instantiate(gameUnitResultUI, unitListParent);
            string unitName = DataManager.Instance.monsterDic[data.Key].name;
            unitResultPanel.Init(unitName, data.Value.spawnCount, data.Value.allDamage);
        }
    }
}
