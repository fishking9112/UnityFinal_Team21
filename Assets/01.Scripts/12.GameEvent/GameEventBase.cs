

using UnityEngine;

public abstract class GameEventBase
{
    public bool IsCompleted { get; protected set; }
    public bool IsFailed { get; protected set; }

    public virtual void StartEvent() { }

    public virtual void UpdateEvent()
    {
        if (!IsCompleted && !IsFailed)
        {
            if (CheckFailureCondition())
            {
                FailEvent();
            }
            else if (CheckCompletionCondition())
            {
                CompleteEvent();
            }
        }
    }

    protected abstract bool CheckCompletionCondition();
    protected abstract bool CheckFailureCondition();

    protected virtual void CompleteEvent()
    {
        Utils.Log("이벤트 완료됨");
        IsCompleted = true;
        GiveReward();
    }

    protected virtual void FailEvent()
    {
        Utils.Log("이벤트 실패됨");
        IsFailed = true;
        OnFail();
    }

    protected virtual void GiveReward() { }
    protected virtual void OnFail() { }
}
