using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 몬스터의 상태 베이스
/// </summary>
public abstract class MonsterBaseState : IState
{
    protected Transform target => stateMachine.Controller.target;
    protected NavMeshAgent navMeshAgent => stateMachine.Controller.navMeshAgent;
    protected Transform pivot => stateMachine.Controller.pivot;
    protected SPUM_Prefabs spum => stateMachine.Controller.spum;
    protected Collider2D collider => stateMachine.Controller._collider;
    protected StatHandler stat => stateMachine.Controller.statHandler;
    protected float targetDistance = float.MaxValue;


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
