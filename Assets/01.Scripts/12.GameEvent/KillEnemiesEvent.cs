using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillEnemiesEvent : GameEventBase
{
    private List<GameObject> heros = new();

    public KillEnemiesEvent(List<GameObject> spawnedHeros)
    {
        this.heros = spawnedHeros;
    }
    public KillEnemiesEvent(GameObject spawnedHero)
    {
        this.heros.Add(spawnedHero);
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
    }

    protected override void OnFail()
    {
        Utils.Log("실패? 없을텐데 왜?");
    }
}
