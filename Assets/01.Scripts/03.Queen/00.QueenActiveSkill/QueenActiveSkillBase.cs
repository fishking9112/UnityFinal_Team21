using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class QueenActiveSkillBase : MonoBehaviour
{
    public QueenActiveSkillInfo info;
    protected QueenController controller;

    public bool onCoolTime;

    public virtual void Init()
    {
        controller = GameManager.Instance.queen.controller;
        onCoolTime = false;
    }

    public async UniTask ApplyCooltimeSkill()
    {
        onCoolTime = true;
        controller.queenActiveSkillSlot.StartCoolTimeUI(controller.selectedSlotIndex, info.coolTime);
        await UniTask.Delay((int)(info.coolTime * 1000));
        onCoolTime = false;
    }

    public async UniTask TryUseSkill()
    {
        if (onCoolTime)
        {
            return;
        }

        GameManager.Instance.queen.condition.AdjustCurQueenActiveSkillGauge(-controller.selectedQueenActiveSkill.info.cost);
        UseSkill();
        await ApplyCooltimeSkill();
        return;
    }

    public abstract void UseSkill();
}