using UnityEngine;

public class SlowBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        buff.particle = ParticleManager.Instance.SpawnParticle("Slow", target.transform.position, new Vector3(0.5f, 0.5f, 1f), Quaternion.identity, target.transform);
        target.statHandler.moveSpeed.AddModifier(ModifierType.Multiply, (int)IDBuff.MOVE_SPEED_UP, 1 + amount);
    }

    public void Remove(BaseController target, Buff buff, BuffInfo info)
    {
        RemoveParticle(buff);
        target.statHandler.moveSpeed.RemoveModifier(ModifierType.Multiply, (int)IDBuff.MOVE_SPEED_UP);
    }
}
