using Cysharp.Threading.Tasks;
using UnityEngine;

public class GiantFormBuff : BaseBuffStrategy, IBuffStrategy
{
    public void Apply(BaseController target, Buff buff, BuffInfo info, float amount)
    {
        target.statHandler.health.AddModifier(ModifierType.Multiply, (int)IDBuff.GIANT_FORM, 1 + amount);
        target.statHandler.attackRange.AddModifier(ModifierType.Multiply, (int)IDBuff.GIANT_FORM, 1 + amount);

        Vector3 giantScale = target.transform.localScale * (1 + amount);
        SmoothSetScale(target, target.transform.localScale, giantScale, 0.5f).Forget();
    }

    public void Remove(BaseController target, Buff buff)
    {
        target.statHandler.health.RemoveModifier(ModifierType.Multiply, (int)IDBuff.GIANT_FORM);
        target.statHandler.attackRange.RemoveModifier(ModifierType.Multiply, (int)IDBuff.GIANT_FORM);

        SmoothSetScale(target, target.transform.localScale, new Vector3(1f, 1f, 1f), 0.5f).Forget();
    }

    private async UniTask SmoothSetScale(BaseController target, Vector3 from, Vector3 to, float duration)
    {
        float time = 0f;

        while (time < duration)
        {
            if (target == null)
            {
                return;
            }

            time += Time.deltaTime;
            float temp = time / duration;
            target.transform.localScale = Vector3.Lerp(from, to, temp);
            await UniTask.Yield();
        }
        target.transform.localScale = to;
    }
}
