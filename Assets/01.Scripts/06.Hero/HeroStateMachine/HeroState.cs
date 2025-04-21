using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using Unity.Android.Types;
using UnityEngine;
using UnityEngine.AI;

public class HeroState : StateMachine
{
    public Hero hero { get; private set; }

    public HeroMoveState moveState { get; private set; }
    public HeroAttackState attackState { get; private set; }
    public HeroDeadStete deadState { get; private set; }

    public NavMeshAgent navMeshAgent { get; set; }

    public Transform target;
    public Vector2 dir { get; set; }
    public float moveSpeed { get; private set; }


    // 강화 관련 추가


    public HeroState(Hero hero)
    {
        this.hero = hero;

        moveState = new HeroMoveState(this);
        attackState = new HeroAttackState(this);
        deadState = new HeroDeadStete(this);

        // 데이터 가져오는걸로 수정 필요
        target = GameObject.Find("Castle").transform;
    }


    public Vector2 GetDir()
    {
        return target.position;
    }
}
