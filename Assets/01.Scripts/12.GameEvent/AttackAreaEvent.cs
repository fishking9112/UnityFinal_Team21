using UnityEngine;

public class AttackAreaEvent : GameEventBase
{
    private EventInfo tableInfo;
    private MiniBarrack barrackInstance;
    public float spawnDuration;
    public float spawnCurrentDuration;
    private Vector2 spawnPosition;



    // 생성자에서 성 프리팹과 위치 전달
    public AttackAreaEvent(MiniBarrack barrackInstance, Vector2 spawnPosition, float spawnDuration, EventInfo eventTableInfo, GameEventContextUI contextUI)
    {
        this.barrackInstance = barrackInstance;
        this.spawnPosition = spawnPosition;
        this.spawnDuration = spawnDuration;
        this.spawnCurrentDuration = spawnDuration;
        this.contextUI = contextUI;
        tableInfo = eventTableInfo;
        this.contextUI.titleText.text = $"◆ {tableInfo.name}";
        UpdateText();
    }

    public override void StartEvent()
    {
        base.StartEvent();

        Utils.Log($"히어로 막사가 소환했습니다. 부셔야 합니다.");
    }

    public override void UpdateEvent()
    {
        if (barrackInstance != null)
        {
            barrackInstance.spawnDurationText.text = $"{Mathf.CeilToInt(spawnCurrentDuration)}s";
            UpdateText();
            spawnCurrentDuration -= Time.deltaTime;

            if (spawnCurrentDuration <= 0f)
            {
                // 소환 및 스폰 타임 초기화
                spawnCurrentDuration = spawnDuration;
                HeroManager.Instance.SummonHeros(spawnPosition, 1, false);//eventTableInfo.createId);
            }
        }

        base.UpdateEvent(); // 성공/실패 판정
    }

    protected override bool CheckCompletionCondition()
    {
        return barrackInstance == null;
    }

    protected override bool CheckFailureCondition()
    {
        return false;
    }

    protected override void GiveReward()
    {
        Utils.Log("배럭을 성공적으로 부셨습니다! 보상 지급!");
        // GameManager.Instance.queen.condition.AdjustCurExpGauge(tableInfo.reward);
        GameManager.Instance.queen.condition.QuestLevelUp(tableInfo.reward);
        GameObject.Destroy(contextUI.gameObject);
        // 보상 지급 로직 추가 가능
    }

    protected override void OnFail()
    {
        Utils.Log("실패? 없을텐데 왜?");
        GameObject.Destroy(contextUI.gameObject);
        // 패널티 로직 추가 가능
    }

    private void UpdateText()
    {
        string tmp = DataManager.Instance.eventDic[tableInfo.ID].description;
        string result = string.Format(tmp, Mathf.CeilToInt(spawnCurrentDuration));
        contextUI.SetText(result);
    }
}