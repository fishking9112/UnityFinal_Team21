using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

interface IHPHandler
{
    float CurrentHP { get; set; }
    float maxHP { get; set; }
    public void SetHP();
}

public class BaseController : MonoBehaviour
{
    private string name;
    private NavMeshAgent navMesh;
    private GameObject target;

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

    protected void DetectEnemy() { }

    protected void AttackTarget() { }

    protected void OnDamaged() { }

    protected void Move() { }

    protected void Die() { }

}
