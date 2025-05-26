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
        await UniTask.Delay((int)(info.coolTime * 1000), false, PlayerLoopTiming.Update);
        onCoolTime = false;
    }

    public async UniTask TryUseSkill(float value)
    {
        if (onCoolTime)
        {
            Utils.Log("대상이 존재하지 않습니다.");
            return;
        }
        if (!RangeCheck())
        {
            Utils.Log("대상이 존재하지 않습니다.");
            return;
        }

        condition.AdjustCurQueenActiveSkillGauge(-value);
        UseSkill();
        controller.selectedQueenActiveSkill = null;
        await ApplyCooltimeSkill();
        return;
    }

    public abstract void UseSkill();

    protected abstract bool RangeCheck();
}