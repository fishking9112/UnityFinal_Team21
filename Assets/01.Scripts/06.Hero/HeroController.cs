using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HeroController : BaseController
{
    public NavMeshAgent navMeshAgent;


    // Start is called before the first frame update
    protected override void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
    }




    public override void TakeDamaged(float damage)
    {
        base.TakeDamaged(damage);
    }

}
