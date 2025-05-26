using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class DecaySkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.DECAY];
    }


    public override async void UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;
        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, info.size, info.target);

        Vector3 scale = new Vector3(info.size / 4, info.size / 4, 1f);
        ParticleObject skillParticle = ParticleManager.Instance.SpawnParticle("Decay_Skill", mousePos, scale, Quaternion.identity);

        List<UniTask> tasks = new List<UniTask>();
        foreach (var hit in hits)
        {
            if (HeroManager.Instance.hero.TryGetValue(hit.gameObject, out var hero))
            {
                hero.TakeDamaged(info.value);
                UniTask decayTask = BuffManager.Instance.ApplyBuff(hero, info.buff_ID, info.buff_Level);
                tasks.Add(decayTask);
            }
        }
        await UniTask.WhenAll(tasks);
    }

    protected override bool RangeCheck()
    {
        return true;
    }
}
