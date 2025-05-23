using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class RecallSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.RECALL];
    }

    public override async void UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;
        Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, info.size, info.target);

        Vector3 scale = new Vector3(info.size / 4, info.size / 4, 1f);
        ParticleObject particle = ParticleManager.Instance.SpawnParticle("Recall", mousePos, scale, Quaternion.identity);

        List<UniTask> tasks = new List<UniTask>();
        Vector3 castlePos = GameManager.Instance.castle.transform.position;

        foreach (var hit in hits)
        {
            if (MonsterManager.Instance.monsters.TryGetValue(hit.gameObject, out var monster))
            {
                tasks.Add(RecallAfterDelay(monster, 0.5f, castlePos));
            }
        }

        await UniTask.WhenAll(tasks);
    }

    private async UniTask RecallAfterDelay(MonsterController monster, float delaySeconds, Vector3 targetPosition)
    {
        await UniTask.Delay((int)(delaySeconds * 1000), false, PlayerLoopTiming.Update);
        monster.transform.position = targetPosition;
    }
}
