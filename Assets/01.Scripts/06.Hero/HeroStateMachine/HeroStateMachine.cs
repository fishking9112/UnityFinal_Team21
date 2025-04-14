using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public void StateEnter();
    public void StateExit();
}

public abstract class HeroStateMachine
{
    protected IState currentState;

    public void ChangeState(IState state)
    {
        currentState?.StateExit();
        currentState = state;
        currentState?.StateEnter();
    }


}
