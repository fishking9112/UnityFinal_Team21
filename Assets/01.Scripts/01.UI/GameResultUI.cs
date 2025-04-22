using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cysharp.Threading.Tasks;

public class GameResultUI : MonoBehaviour
{
    public GameObject resultWindow;
    public TextMeshProUGUI gameTimeText;
    public TextMeshProUGUI resourceText;


    public Transform unitListParent;
    public GameUnitResultUI gameUnitResultUI;
    public IngameUI ingameUI;


    /// <summary>   
    /// 시작 시 UI를 초기화합니다.
    /// </summary>
    private void Start()
    {
        GameResultManager.Instance.SetUI(this);
    }

    public void OnClickTitleMenu()
    {
        GameResultManager.Instance.ReturnToTitle();
    }


    public void ShowUnitResult()
    {
        gameTimeText.text = Utils.GetMMSSTime((int)(1800f - ingameUI.GetTimer())); // TODO : 1800f 수정 필요 (UI 리펙토링 하면서 수정)
        resourceText.text = GameManager.Instance.queen.condition.Gold.Value.ToString();// + GameManager.Instance.GetGold();

        foreach (var data in GameResultManager.Instance.resultDatas)
        {
            GameUnitResultUI unitResultPanel = Instantiate(gameUnitResultUI, unitListParent);
            string unitName = DataManager.Instance.monsterDic[data.Key].name;
            unitResultPanel.Init(unitName, data.Value.spawnCount, data.Value.allDamage);
        }
    }
}
