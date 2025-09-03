using Cysharp.Threading.Tasks;
using UnityEngine;

public class ManaRecycleSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.MANA_RECYCLE];
    }

    public override UniTask UseSkill()
    {
        Vector3 targetScale = GameManager.Instance.castle.transform.localScale;
        Vector3 particlePos = GameManager.Instance.castle.transform.position;
        Vector3 particleScale = targetScale * 1.5f;

        ParticleManager.Instance.SpawnParticle("ManaRecycle", particlePos, particleScale);
        condition.AdjustCurSummonGauge(info.value);

        return UniTask.CompletedTask;
    }

    protected override bool RangeCheck()
    {
        return true;
    }
}