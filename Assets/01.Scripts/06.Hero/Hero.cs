using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Hero : MonoBehaviour, IPoolable
{
    private HeroState stateMachine;

    public  HeroController controller;
    public GameObject target;

    private int enemyLayer;

    private void Awake()
    {
        GameManager.Instance.hero = this;
    }

    private void Start()
    {
        stateMachine = new HeroState(this);
        controller=GetComponent<HeroController>();
        stateMachine.navMeshAgent = controller.navMeshAgent;
        stateMachine.ChangeState(stateMachine.moveState);

        enemyLayer = LayerMask.GetMask("Monster");

        DeadCheck().Forget();
    }

    public GameObject FindNearestTarget()
    {
        target = null;

        Vector2 pointA = Camera.main.ScreenToWorldPoint(Vector3.zero);
        Vector2 pointB = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height)); ;

        Vector2 off = pointA - pointB;

        pointA = (Vector2)transform.position - off / 2;
        pointB = (Vector2)transform.position + off / 2;

        Collider2D[] col = Physics2D.OverlapAreaAll(pointA, pointB, enemyLayer);

        if (col.Length == 0)
            return null;

        float minVal = float.MaxValue;

        foreach (Collider2D c in col)
        {
            float distance=Vector2.Distance(c.transform.position, transform.position);
            if(minVal>distance)
            {
                minVal = distance;
                target = c.gameObject;
            }
        }

        Debug.Log(target);

        return target;
    }

    public void Init(Action<GameObject> returnAction)
    { 
        throw new NotImplementedException();
    }

    public void OnSpawn()
    {
        stateMachine = new HeroState(this);
        stateMachine.ChangeState(stateMachine.moveState);

        enemyLayer = LayerMask.GetMask("Monster");

        DeadCheck().Forget();
    }
    
    private async UniTaskVoid DeadCheck()
    {
        // 사망 체크로 수정 핋요
        await UniTask.WaitUntil(() => gameObject.activeSelf == false);
        stateMachine.ChangeState(stateMachine.deadState);
    }

    public void OnDespawn()
    {
        throw new NotImplementedException();
    }
}
