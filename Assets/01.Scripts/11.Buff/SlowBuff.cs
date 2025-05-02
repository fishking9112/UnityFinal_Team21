using UnityEngine;

public class SlowBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        buff.particle = ParticleManager.Instance.SpawnParticle("Slow", target.transform.position, Quaternion.identity, 0.5f, target.transform);
        target.MoveSpeedBuff(amount);
    }

    public void Remove(BaseController target, Buff buff, BuffInfo info)
    {
        RemoveParticle(buff);
        target.EndMoveSpeedBuff();
    }
}
