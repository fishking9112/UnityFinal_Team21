using UnityEngine;

public class DecayBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        buff.particle = ParticleManager.Instance.SpawnParticle("Decay_Buff", target.transform.position + Vector3.up, new Vector3(0.1f, 0.1f, 1f), Quaternion.identity, target.transform);
        target.statHandler.addAttack.AddModifier(ModifierType.Plus, (int)IDBuff.DECAY, amount);
    }

    public void Remove(BaseController target, Buff buff)
    {
        RemoveParticle(buff);
        target.statHandler.addAttack.RemoveModifier(ModifierType.Plus, (int)IDBuff.DECAY);
    }
}