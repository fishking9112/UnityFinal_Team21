using System.Collections;
using System.Collections.Generic;
using Unity.Play.Publisher.Editor;
using UnityEngine;

public class DefendAreaEvent : GameEventBase
{
    private EventTableInfo tableInfo;
    private MiniCastle castleInstance;
    private float defendDuration = 5f;
    private Vector2 spawnPosition;

    // 생성자에서 성 프리팹과 위치 전달
    public DefendAreaEvent(MiniCastle castleInstance, Vector2 spawnPosition, float defendDuration, EventTableInfo eventTableInfo, GameEventContextUI contextUI)
    {
        this.castleInstance = castleInstance;
        this.spawnPosition = spawnPosition;
        this.defendDuration = defendDuration;
        this.contextUI = contextUI;
        tableInfo = eventTableInfo;
        this.contextUI.titleText.text = $"◆ {eventTableInfo.name}";
        UpdateText();
    }

    public override void StartEvent()
    {
        base.StartEvent();

        Utils.Log($"성을 소환했습니다. {defendDuration}초 동안 지켜야 합니다.");
    }

    public override void UpdateEvent()
    {
        if (castleInstance == null)
        {
            // 성이 파괴된 경우
            IsFailed = true;
            OnFail();
            return;
        }

        if (castleInstance != null)
        {
            castleInstance.timerText.text = $"{Mathf.CeilToInt(defendDuration)}s";
            UpdateText();
            defendDuration -= Time.deltaTime;
        }

        base.UpdateEvent(); // 성공/실패 판정
    }

    protected override bool CheckCompletionCondition()
    {
        return castleInstance != null && 0f >= defendDuration;
    }

    protected override bool CheckFailureCondition()
    {
        return castleInstance == null;
    }

    protected override void GiveReward()
    {
        Utils.Log("성을 성공적으로 방어했습니다! 보상 지급!");
        GameManager.Instance.queen.condition.AdjustCurExpGauge(tableInfo.reward);
        GameObject.Destroy(castleInstance.gameObject);
        GameObject.Destroy(contextUI.gameObject);
        // 보상 지급 로직 추가 가능
    }

    protected override void OnFail()
    {
        Utils.Log("성이 파괴되었습니다! 방어 실패!");
        GameObject.Destroy(contextUI.gameObject);
        // 패널티 로직 추가 가능
    }

    private void UpdateText()
    {
        string tmp = DataManager.Instance.eventDic[tableInfo.ID].description;
        string result = string.Format(tmp, Mathf.CeilToInt(defendDuration));
        contextUI.SetText(result);
    }
}