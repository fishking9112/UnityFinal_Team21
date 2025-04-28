using UnityEngine;

public class MonsterHealSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[204];
    }

    public override void UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;

        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, info.size, info.target);

        foreach (var hit in hits)
        {
            if(MonsterManager.Instance.monsters.TryGetValue(hit.gameObject,out var monster))
            {
                monster.Heal(info.value);
            }
        }
    }
}
