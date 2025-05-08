using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class WarcrySkill : QueenActiveSkillBase
{
    public override void Init()
    {
        //base.Init();

        //info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.ATTACK_DAMAGE_UP];
    }

    public override async void UseSkill()
    {
        // 마우스 위치를 기준으로 size만큼 충돌 검사
        Vector3 mousePos = controller.worldMousePos;
        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, 10f, LayerMask.GetMask("Monster"));

        // 충돌한 모든 몬스터에게 버프 적용
        List<UniTask> tasks = new List<UniTask>();
        foreach (var hit in hits)
        {
            if (MonsterManager.Instance.monsters.TryGetValue(hit.gameObject, out var monster))
            {
                ParticleObject skillParticle = ParticleManager.Instance.SpawnParticle("AttackDMG_Light", monster.transform.position + new Vector3(0, 0.1f, 0), new Vector3(0.1f, 0.1f, 1f), Quaternion.identity, monster.transform);

                ParticleObject buffParticle = ParticleManager.Instance.SpawnParticle("AttackDMG_Sword", monster.transform.position + new Vector3(0, 0.5f, 0), new Vector3(1f, 1f, 1f), Quaternion.identity, monster.transform);
                BuffCounter counter = new BuffCounter(1, () => { buffParticle.OnDespawn(); });
                UniTask attackDamageTask = BuffManager.Instance.ApplyBuff(monster, (int)IDBuff.ATTACK_DAMAGE_UP, 1, counter.BuffEnd);
                tasks.Add(attackDamageTask);
            }
        }
        await UniTask.WhenAll(tasks);
    }
}