using Cysharp.Threading.Tasks;
using UnityEngine;

public class MilitiaSummonSkill : QueenActiveSkillBase
{
    public float summonRadius = 10f;

    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.SUMMON_MILITIA];
    }

    public override UniTask UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;

        for (int i = 0; i < info.value; i++)
        {
            float angle = (360f / info.value) * i * Mathf.Deg2Rad;

            Vector3 spawnPos = new Vector3(Mathf.Cos(angle) * summonRadius, Mathf.Sin(angle) * summonRadius, 0f) + GameManager.Instance.castle.transform.position;

            MonsterInfo monsterInfo = MonsterManager.Instance.monsterInfoList[info.monster_ID];
            var summonMonster = ObjectPoolManager.Instance.GetObject<MonsterController>(monsterInfo.outfit, spawnPos);
            summonMonster.StatInit(monsterInfo, MonsterManager.Instance.isHealthUI,info.summon_Time);
        }

        return UniTask.CompletedTask;
    }

    protected override bool RangeCheck()
    {
        return true;
    }
}
