using UnityEngine;

public class ZombieSummonSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.SUMMON_ZOMBIE];
    }

    public override void UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;
        string randZombie = Random.Range(0, 2) == 1 ? "Zombie_0" : "Zombie_1";

        var zombie = ObjectPoolManager.Instance.GetObject<MonsterController>(randZombie, mousePos);
        zombie.StatInit(MonsterManager.Instance.monsterInfoList[(int)IDMonster.SKELETON_NORMAL], MonsterManager.Instance.isHealthUI);
    }

    protected override bool RangeCheck()
    {
        return true;
    }
}