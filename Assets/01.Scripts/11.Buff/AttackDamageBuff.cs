using UnityEngine;

public class AttackDamageBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        buff.particle = ParticleManager.Instance.SpawnParticle("AttackDMG_Sword", target.transform.position + Vector3.up, Quaternion.identity, 0.5f, target.transform);
        target.AttackDamageBuff(amount);
    }

    public void Remove(BaseController target, Buff buff, BuffInfo info)
    {
        RemoveParticle(buff);
        target.EndAttackDamageBuff();
    }
}