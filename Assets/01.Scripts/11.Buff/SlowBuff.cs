using UnityEngine;

public class SlowBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        target.MoveSpeedBuff(amount);
    }

    public void Remove(BaseController target, Buff buff, BuffInfo info)
    {
        target.EndMoveSpeedBuff();
    }
}
