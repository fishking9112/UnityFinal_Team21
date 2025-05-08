using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;


public class MonsterAttackBuffSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.ATTACK_DAMAGE_UP];
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
                ParticleObject skillParticle = ParticleManager.Instance.SpawnParticle("AttackDMG_Light", monster.transform.position + new Vector3(0, 0.1f, 0), new Vector3(0.1f, 0.1f, 1f), Quaternion.identity, monster.transform);

                ParticleObject buffParticle = ParticleManager.Instance.SpawnParticle("AttackDMG_Sword", monster.transform.position + new Vector3(0, 0.7f, 0), new Vector3(0.3f, 0.3f, 0.3f), Quaternion.identity, monster.transform);
                BuffParticleController particleController = new BuffParticleController(1, () => { buffParticle.OnDespawn(); });
                UniTask attackDamageTask = BuffManager.Instance.ApplyBuff(monster, info.buff_ID, info.buff_Level, particleController);
                tasks.Add(attackDamageTask);
            }
        }
        await UniTask.WhenAll(tasks);
    }
}
