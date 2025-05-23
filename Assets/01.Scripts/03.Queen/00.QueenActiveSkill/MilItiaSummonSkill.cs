using UnityEngine;

public class MilitiaSummonSkill : QueenActiveSkillBase
{
    public float summonRadius = 10f;

    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.SUMMON_MILITIA];
    }

    public override void UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;

        for (int i = 0; i < info.value; i++)
        {
            float angle = (360f / info.value) * i * Mathf.Deg2Rad;

            Vector3 spawnPos = new Vector3(Mathf.Cos(angle) * summonRadius, Mathf.Sin(angle) * summonRadius, 0f) + GameManager.Instance.castle.transform.position;

            MonsterInfo monsterInfo = MonsterManager.Instance.monsterInfoList[info.monster_ID];
            var summonMonster = ObjectPoolManager.Instance.GetObject<MonsterController>(monsterInfo.outfit, spawnPos);
            summonMonster.StatInit(monsterInfo, MonsterManager.Instance.isHealthUI);

            if (MonsterManager.Instance.monsters.TryGetValue(summonMonster.gameObject, out var monster))
            {
                _ = BuffManager.Instance.ApplyBuff(monster, info.buff_ID, info.buff_Level);
            }
        }
    }
}
