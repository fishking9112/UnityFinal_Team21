using UnityEngine;

public class AttackDamageBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        buff.particle = ParticleManager.Instance.SpawnParticle("AttackDMG_Sword", target.transform.position + Vector3.up, new Vector3(0.5f, 0.5f, 1f), Quaternion.identity, target.transform);
        target.statHandler.attack.AddModifier(ModifierType.Multiply, (int)IDBuff.ATTACK_DAMAGE_UP, 1 + amount);
    }

    public void Remove(BaseController target, Buff buff)
    {
        RemoveParticle(buff);
        target.statHandler.attack.RemoveModifier(ModifierType.Multiply, (int)IDBuff.ATTACK_DAMAGE_UP);
    }
}