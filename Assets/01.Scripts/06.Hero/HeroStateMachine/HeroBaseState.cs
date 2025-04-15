using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HeroBaseState : IState
{
    protected HeroState state;

    protected Hero hero;

    public HeroBaseState(HeroState state)
    {
        this.state = state;
        hero = GameManager.Instance.hero;
    }

    public virtual void StateEnter()
    {

    }

    public virtual void StateExit()
    {

    }

}
