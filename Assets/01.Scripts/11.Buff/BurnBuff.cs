using UnityEngine;

public class BurnBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        buff.particle = ParticleManager.Instance.SpawnParticle("Burn", target.transform.position, Quaternion.identity, 0.5f, target.transform);
        _ = TakeTickDamaged(target, info, buff.token, buff.level, amount);
    }

    public void Remove(BaseController target, Buff buff, BuffInfo info)
    {
        RemoveParticle(buff);
    }
}