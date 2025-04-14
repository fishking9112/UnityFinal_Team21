using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HeroBaseState : IState
{
    protected HeroState state;

    private Hero hero;

    protected virtual async UniTaskVoid DeadCheck(CancellationToken token)
    {
        // 사망 체크로 수정 핋요
        await UniTask.WaitUntil(() =>hero.enabled==false,PlayerLoopTiming.Update,token);
        state.ChangeState(state.deadState);
    }

    public virtual void StateEnter()
    {

    }

    public virtual void StateExit()
    {

    }

}
