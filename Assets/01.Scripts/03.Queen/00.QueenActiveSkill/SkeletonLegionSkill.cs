using UnityEngine;

public class SkeletonLegionSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.SKELETON_LEGION];
    }

    public override void UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;

        for (int i = 0; i < info.value; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * info.size;
            Vector3 spawnPos = mousePos + (Vector3)randomOffset;

            MonsterInfo monsterInfo = MonsterManager.Instance.monsterInfoList[info.monster_ID];
            var summonMonster = ObjectPoolManager.Instance.GetObject<MonsterController>(monsterInfo.outfit, spawnPos);
            summonMonster.StatInit(monsterInfo, MonsterManager.Instance.isHealthUI);
        }
    }
}
