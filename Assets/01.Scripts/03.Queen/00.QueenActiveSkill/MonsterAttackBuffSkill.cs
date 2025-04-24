using UnityEngine;


public class MonsterAttackBuffSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[12];
    }

    public override void UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;

        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, info.size, info.target);

        foreach (var hit in hits)
        {
            Utils.Log("충돌 " + hit.name);
            // 몬스터 공격력 증가 버프
        }
    }
}
