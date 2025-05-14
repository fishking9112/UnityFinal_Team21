using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class LaserSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.LASER];
    }

    public override async void UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;
        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, info.size, info.target);

        List<UniTask> tasks = new List<UniTask>();
        foreach (var hit in hits)
        {
            if (HeroManager.Instance.hero.TryGetValue(hit.gameObject, out var hero))
            {
                ParticleManager.Instance.SpawnParticle("Laser", hero.transform.position, new Vector3(0.5f, 0.5f, 1f), Quaternion.identity, hero.transform);
                hero.TakeDamaged(info.value);
                UniTask slowTask = BuffManager.Instance.ApplyBuff(hero, info.buff_ID, info.buff_Level);
                tasks.Add(slowTask);
            }
        }
        await UniTask.WhenAll(tasks);
    }
}
