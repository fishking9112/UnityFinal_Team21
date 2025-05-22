using System.Threading;
using UnityEngine;

public class AttackDamageAndMoveSpeedAndAttackSpeedBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        Vector3 targetScale = target.transform.localScale;
        Vector3 particlePos = target.transform.position;
        Vector3 particleScale = targetScale * 0.1f;

        buff.particle = ParticleManager.Instance.SpawnParticle("BloodRoar_Buff", particlePos, particleScale, Quaternion.identity, target.transform);
        target.statHandler.attack.AddModifier(ModifierType.Multiply, (int)IDBuff.ATTACK_DAMAGE_AND_MOVE_SPEED_AND_ATTACK_SPEED_UP, 1 + amount);
        target.statHandler.moveSpeed.AddModifier(ModifierType.Multiply, (int)IDBuff.ATTACK_DAMAGE_AND_MOVE_SPEED_AND_ATTACK_SPEED_UP, 1 + amount);
        target.statHandler.attackSpeed.AddModifier(ModifierType.Multiply, (int)IDBuff.ATTACK_DAMAGE_AND_MOVE_SPEED_AND_ATTACK_SPEED_UP, 1 + amount);
    }

    public void Remove(BaseController target, Buff buff)
    {
        RemoveParticle(buff);
        target.statHandler.attack.RemoveModifier(ModifierType.Multiply, (int)IDBuff.ATTACK_DAMAGE_AND_MOVE_SPEED_AND_ATTACK_SPEED_UP);
        target.statHandler.moveSpeed.RemoveModifier(ModifierType.Multiply, (int)IDBuff.ATTACK_DAMAGE_AND_MOVE_SPEED_AND_ATTACK_SPEED_UP);
        target.statHandler.attackSpeed.RemoveModifier(ModifierType.Multiply, (int)IDBuff.ATTACK_DAMAGE_AND_MOVE_SPEED_AND_ATTACK_SPEED_UP);
    }
}
