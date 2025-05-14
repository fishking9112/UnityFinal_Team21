using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class GiantFormBuffSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.GIANT_FORM];
    }

    public override async void UseSkill()
    {
        // 마우스 위치를 기준으로 size만큼 충돌 검사
        Vector3 mousePos = controller.worldMousePos;
        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, info.size, info.target);

        // 충돌한 모든 몬스터에게 버프 적용
        List<UniTask> tasks = new List<UniTask>();
        foreach (var hit in hits)
        {
            if (MonsterManager.Instance.monsters.TryGetValue(hit.gameObject, out var monster))
            {
                UniTask giantFormTask = BuffManager.Instance.ApplyBuff(monster, info.buff_ID, info.buff_Level);
                tasks.Add(giantFormTask);
            }
        }
        await UniTask.WhenAll(tasks);
    }
}
