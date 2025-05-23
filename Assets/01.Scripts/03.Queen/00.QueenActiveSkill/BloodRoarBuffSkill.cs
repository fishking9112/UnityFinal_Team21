using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class BloodRoarBuffSkill : QueenActiveSkillBase
{
    // Start is called before the first frame update
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.BLOOD_ROAR];
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
                monster.healthHandler.Damage(monster.healthHandler.GetCurHP() * info.value);
                UniTask warcryTask = BuffManager.Instance.ApplyBuff(monster, info.buff_ID, info.buff_Level);
                tasks.Add(warcryTask);
            }
        }
        await UniTask.WhenAll(tasks);
    }
}
