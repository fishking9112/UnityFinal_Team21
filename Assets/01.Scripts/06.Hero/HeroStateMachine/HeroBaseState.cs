using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HeroBaseState : IState
{
    protected HeroState state;

    private Hero hero;

    public HeroBaseState(HeroState state)
    {
        this.state = state;
        hero = GameManager.Instance.hero;
    }

    public virtual void Enter()
    {

    }

    public virtual void Exit()
    {

    }

    public virtual void Update()
    {

    }
    public virtual void FixedUpdate()
    {

    }

}
