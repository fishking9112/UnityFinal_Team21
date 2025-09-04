using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public abstract class QueenActiveSkillBase : MonoBehaviour
{
    public QueenActiveSkillInfo info;
    protected QueenController controller;
    protected QueenCondition condition;

    public bool onCoolTime;


    private CancellationTokenSource _cooltimeToken;

    public virtual void Init()
    {
        controller = GameManager.Instance.queen.controller;
        condition = GameManager.Instance.queen.condition;
        onCoolTime = false;
    }

    public async UniTask ApplyCooltimeSkill()
    {
        // 이미 쿨타임 돌고 있으면 중복 방지
        if (onCoolTime)
        {
            return;
        }

        onCoolTime = true;

        _cooltimeToken?.Cancel();
        _cooltimeToken = new CancellationTokenSource();

        controller.queenActiveSkillSlot.StartCoolTimeUI(controller.selectedSlotIndex, info.coolTime);

        try
        {
            await UniTask.Delay(
                (int)(info.coolTime * 1000),
                cancellationToken: _cooltimeToken.Token,
                delayTiming: PlayerLoopTiming.Update
            );
        }
        catch (System.OperationCanceledException)
        {
            // 취소 됐을 경우
            return;
        }
        finally
        {
            onCoolTime = false;
        }
    }

    public async UniTask TryUseSkill(float value)
    {
        if (onCoolTime)
        {
            Utils.Log("쿨타임 입니다.");
            return;
        }
        if (!RangeCheck())
        {
            Utils.Log("대상이 존재하지 않습니다.");
            return;
        }

        condition.AdjustCurQueenActiveSkillGauge(-value);
        _ = ApplyCooltimeSkill();
        UseSkillAfter(); // 스킬 시전과 동시에 업적에 등록되어야함
        controller.selectedQueenActiveSkill = null; // 스킬 시전과 동시에 선택한 스킬이 사라짐

        await UseSkill();
    }

    public abstract UniTask UseSkill();
    public void UseSkillAfter()
    {
        TrophyManager.Instance.UseSkillId(info.id);
    }
    protected abstract bool RangeCheck();
}