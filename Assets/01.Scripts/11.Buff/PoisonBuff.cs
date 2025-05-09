using UnityEngine;

public class PoisonBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        _ = TakeTickDamaged(target, info, buff.token, buff.level, amount);
    }

    public void Remove(BaseController target, Buff buff, BuffInfo info)
    {

    }
}
