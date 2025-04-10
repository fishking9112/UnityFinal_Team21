using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    private Vector2 dir;

    private float randomDelay;
    private float moveSpeed;

    public GameObject target;

    private int enemyLayer;

    // Start is called before the first frame update
    void Start()
    {
        randomDelay = 3f;
        moveSpeed = 5;
        enemyLayer= LayerMask.GetMask("Enemy");

        //MVP용 임시 기능
        ChangeDir().Forget();
        //Move().Forget();
    }

    private async UniTaskVoid Move()
    {
        while(true)
        {
            transform.position = moveSpeed * Time.deltaTime * (Vector3)dir + transform.position;
            await UniTask.Yield();
        }
    }

    private async UniTaskVoid ChangeDir()
    {
        while(true)
        {
            dir = UnityEngine.Random.insideUnitCircle.normalized;
            await UniTask.Delay(TimeSpan.FromSeconds(randomDelay));
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var a= gameObject.GetOrAddComponent<HeroAbilityMissile>();
            a.Init();
        }
        if(Input.GetKeyDown(KeyCode.Space))
        {
            FindNearestTarget();
        }

    }



    public GameObject FindNearestTarget()
    {
        target = null;

        Vector2 pointA=new Vector2(-Screen.width/2,Screen.height/2);
        Vector2 pointB=new Vector2(Screen.width/2,-Screen.height/2);


        Collider2D[] col = Physics2D.OverlapAreaAll(pointA, pointB, enemyLayer);

        //foreach(Collider2D c in col)
        //{

        //}

        target = col[0]?.gameObject;

        return target;
    }

    private void OnDrawGizmos()
    {
        Vector2 pointA = new Vector2(transform.position.x - 1920 / 2, transform.position.y + 1080 / 2);
        Vector2 pointB = new Vector2(transform.position.x + 1920 / 2, transform.position.y - 1080 / 2);
    

        Gizmos.DrawLine(pointA, pointB);
    }
}
