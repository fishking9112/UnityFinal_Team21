using UnityEngine;

public class BurnBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        Vector3 targetScale = target.transform.localScale;
        Vector3 particlePos = target.transform.position + new Vector3(0, targetScale.y * 0.5f, 0);
        Vector3 particleScale = targetScale * 0.1f;

        buff.particle = ParticleManager.Instance.SpawnParticle("Burn", particlePos, particleScale, Quaternion.identity, target.transform);
        _ = TakeTickDamaged(target, info, buff.token, buff.level, amount);
    }

    public void Remove(BaseController target, Buff buff)
    {
        RemoveParticle(buff);
    }
}