using UnityEngine;

public class SkeletonLegionSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        //info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.SUMMON];
    }

    public override void UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;

        for (int i = 0; i < info.value; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * info.size;
            Vector3 spawnPos = mousePos + (Vector3)randomOffset;

            var skeleton = ObjectPoolManager.Instance.GetObject<MonsterController>("Skeleton_Normal", spawnPos);
            skeleton.StatInit(MonsterManager.Instance.monsterInfoList[(int)IDMonster.SKELETON_NORMAL], MonsterManager.Instance.isHealthUI);
        }
    }
}
