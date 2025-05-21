using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEnemiesEvent : GameEventBase
{
    private EventTableInfo tableInfo;
    private List<GameObject> heros = new();
    private int maxCount = 0;
    private int curCount = 0;

    public KillEnemiesEvent(List<GameObject> spawnedHeros, EventTableInfo eventTableInfo, GameEventContextUI contextUI)
    {
        this.heros = spawnedHeros;
        maxCount = heros.Count;
        this.contextUI = contextUI;
        tableInfo = eventTableInfo;
        contextUI.titleText.text = $"◆ {tableInfo.name}";
        UpdateText();
    }
    public KillEnemiesEvent(GameObject spawnedHero, EventTableInfo eventTableInfo, GameEventContextUI contextUI)
    {
        this.heros.Add(spawnedHero);
        maxCount = heros.Count;
        this.contextUI = contextUI;
        tableInfo = eventTableInfo;
        contextUI.titleText.text = $"◆ {tableInfo.name}";
        UpdateText();
    }

    public override void StartEvent()
    {
        base.StartEvent();
        // 이미 외부에서 몬스터 생성했다고 가정
    }

    public override void UpdateEvent()
    {
        base.UpdateEvent();
    }

    // Hero가 비활성화 되면 하나씩 제거
    protected override bool CheckCompletionCondition()
    {
        heros.RemoveAll(e => e.gameObject.activeSelf == false);

        if (maxCount - heros.Count != curCount)
        {
            curCount = maxCount - heros.Count;
            UpdateText();
        }

        return heros.Count == 0;
    }

    // 실패가 없는 이벤트
    protected override bool CheckFailureCondition()
    {
        return false;
    }

    protected override void GiveReward()
    {
        Utils.Log("모든 몬스터 처치 완료! 보상 지급!");
        GameObject.Destroy(contextUI.gameObject);
        GameManager.Instance.queen.condition.AdjustCurExpGauge(tableInfo.reward);
    }

    protected override void OnFail()
    {
        GameObject.Destroy(contextUI.gameObject);
        Utils.Log("실패? 없을텐데 왜?");
    }

    private void UpdateText()
    {
        string tmp = DataManager.Instance.eventDic[tableInfo.ID].description;
        string result = string.Format(tmp, curCount, maxCount);
        contextUI.SetText(result);
    }
}
