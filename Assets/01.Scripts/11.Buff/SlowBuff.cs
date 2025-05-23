using UnityEngine;

public class SlowBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        Vector3 targetScale = target.transform.localScale;
        Vector3 particlePos = target.transform.position;
        Vector3 particleScale = targetScale * 0.5f;

        buff.particle = ParticleManager.Instance.SpawnParticle("Slow", particlePos, particleScale, Quaternion.identity, target.transform);
        target.statHandler.moveSpeed.AddModifier(ModifierType.Multiply, (int)IDBuff.MOVE_SPEED_UP, 1 + amount);
    }

    public void Remove(BaseController target, Buff buff)
    {
        RemoveParticle(buff);
        target.statHandler.moveSpeed.RemoveModifier(ModifierType.Multiply, (int)IDBuff.MOVE_SPEED_UP);
    }
}
