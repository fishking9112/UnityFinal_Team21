using UnityEngine;

public class AttackDamageAndAttackSpeedBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        buff.particle = ParticleManager.Instance.SpawnParticle("Warcry_Buff", target.transform.position, new Vector3(0.1f, 0.1f, 1f), Quaternion.identity, target.transform);
        target.statHandler.attack.AddModifier(ModifierType.Multiply, (int)IDBuff.ATTACK_DAMAGE_AND_ATTACK_SPEED_UP, 1 + amount);
        target.statHandler.attackSpeed.AddModifier(ModifierType.Multiply, (int)IDBuff.ATTACK_DAMAGE_AND_ATTACK_SPEED_UP, 1 + amount);
    }

    public void Remove(BaseController target, Buff buff)
    {
        RemoveParticle(buff);
        target.statHandler.attack.RemoveModifier(ModifierType.Multiply, (int)IDBuff.ATTACK_DAMAGE_AND_ATTACK_SPEED_UP);
        target.statHandler.attackSpeed.RemoveModifier(ModifierType.Multiply, (int)IDBuff.ATTACK_DAMAGE_AND_ATTACK_SPEED_UP);
    }
}