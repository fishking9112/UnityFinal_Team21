using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BaseController : MonoBehaviour
{
    private NavMeshAgent navMesh;
    private GameObject target;


    protected HealthHandler healthHandler;

    private float attack;
    public float Attack
    {
        get { return attack; }
        private set { attack = value; }
    }

    private float def;
    public float Def
    {
        get { return def; }
        private set { def = value; }
    }

    private float moveSpeed;
    public float MoveSpeed
    {
        get { return moveSpeed; }
        private set { moveSpeed = value; }
    }

    private float attackDelay;
    public float AttackDelay
    {
        get { return attackDelay; }
        private set { attackDelay = value; }
    }

    protected virtual void DetectTarget() { }

    protected virtual void AttackTarget() { }

    protected virtual void OnDamaged() { }

    protected virtual void Move() { }

    protected virtual void Die() { }

}
