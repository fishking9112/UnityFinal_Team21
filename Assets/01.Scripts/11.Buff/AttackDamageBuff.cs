using UnityEngine;

public class AttackDamageBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        target.AttackDamageBuff(amount);
    }

    public void Remove(BaseController target, Buff buff, BuffInfo info)
    {
        target.EndAttackDamageBuff();
    }
}