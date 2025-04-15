using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class MonsterBaseState : IState
{
    protected Transform target => stateMachine.Controller.target;
    protected NavMeshAgent navMeshAgent => stateMachine.Controller.navMeshAgent;
    protected float targetDistance = float.MaxValue;
    protected SpriteRenderer sprite => stateMachine.Controller.sprite;


    protected readonly MonsterStateMachine stateMachine;

    public MonsterBaseState(MonsterStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }


    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void FixedUpdate() { }
    public virtual void Update() { }
}
