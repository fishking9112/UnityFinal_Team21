using System.Threading;
using UnityEngine;

public class AttackDamageBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        Vector3 targetScale = target.transform.localScale;
        Vector3 particlePos = target.transform.position + new Vector3(0, targetScale.y * 1f, 0);
        Vector3 particleScale = targetScale * 0.5f;

        buff.particle = ParticleManager.Instance.SpawnParticle("AttackDMG_Sword", particlePos, particleScale, Quaternion.identity, target.transform);
        target.statHandler.attack.AddModifier(ModifierType.Multiply, (int)IDBuff.ATTACK_DAMAGE_UP, 1 + amount);
    }

    public void Remove(BaseController target, Buff buff)
    {
        RemoveParticle(buff);
        target.statHandler.attack.RemoveModifier(ModifierType.Multiply, (int)IDBuff.ATTACK_DAMAGE_UP);
    }
}