using UnityEngine;

public class SlowBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        target.statHandler.moveSpeed.AddModifier(ModifierType.Multiply, (int)IDBuff.MOVE_SPEED_UP, 1 + amount);
    }

    public void Remove(BaseController target, Buff buff, BuffInfo info)
    {
        target.statHandler.moveSpeed.RemoveModifier(ModifierType.Multiply, (int)IDBuff.MOVE_SPEED_UP);
    }
}
