using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class HeroBaseState : IState
{
    protected HeroState state;
    protected StatHandler stat => state.controller.statHandler;
    protected NavMeshAgent navMeshAgent => state.controller.navMeshAgent;

    public HeroBaseState(HeroState state)
    {
        this.state = state;
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
