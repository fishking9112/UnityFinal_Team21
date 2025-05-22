using UnityEngine;

public class DecayBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        Vector3 targetScale = target.transform.localScale;
        Vector3 particlePos = target.transform.position + new Vector3(0, targetScale.y * 1f, 0);
        Vector3 particleScale = targetScale * 0.1f;

        buff.particle = ParticleManager.Instance.SpawnParticle("Decay_Buff", particlePos, particleScale, Quaternion.identity, target.transform);
        target.statHandler.addAttack.AddModifier(ModifierType.Plus, (int)IDBuff.DECAY, amount);
    }

    public void Remove(BaseController target, Buff buff)
    {
        RemoveParticle(buff);
        target.statHandler.addAttack.RemoveModifier(ModifierType.Plus, (int)IDBuff.DECAY);
    }
}