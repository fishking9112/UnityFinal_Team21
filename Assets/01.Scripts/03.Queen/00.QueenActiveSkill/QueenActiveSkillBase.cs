using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class QueenActiveSkillBase : MonoBehaviour
{
    public QueenActiveSkillInfo info;
    protected QueenController controller;
    protected QueenCondition condition;

    public bool onCoolTime;

    public virtual void Init()
    {
        controller = GameManager.Instance.queen.controller;
        condition = GameManager.Instance.queen.condition;
        onCoolTime = false;
    }

    public async UniTask ApplyCooltimeSkill()
    {
        onCoolTime = true;
        controller.queenActiveSkillSlot.StartCoolTimeUI(controller.selectedSlotIndex, info.coolTime);
        await UniTask.Delay((int)(info.coolTime * 1000));
        onCoolTime = false;
    }

    public async UniTask TryUseSkill(float value)
    {
        if (onCoolTime)
        {
            return;
        }

        condition.AdjustCurQueenActiveSkillGauge(-value);
        UseSkill();
        await ApplyCooltimeSkill();
        return;
    }

    public abstract void UseSkill();
}