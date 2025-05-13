using UnityEngine;

public class PoisonBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        buff.particle = ParticleManager.Instance.SpawnParticle("Poison", target.transform.position, new Vector3(0.5f, 0.5f, 1f), Quaternion.identity, target.transform);
        _ = TakeTickDamaged(target, info, buff.token, buff.level, amount);
    }

    public void Remove(BaseController target, Buff buff)
    {
        RemoveParticle(buff);
    }
}
