using UnityEngine;

public class BurnBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        buff.particle = ParticleManager.Instance.SpawnParticle("Burn", target.transform.position + new Vector3(0, 0.5f, 0), new Vector3(0.1f, 0.1f, 1f), Quaternion.identity, target.transform);
        _ = TakeTickDamaged(target, info, buff.token, buff.level, amount);
    }

    public void Remove(BaseController target, Buff buff, BuffInfo info)
    {
        RemoveParticle(buff);
    }
}