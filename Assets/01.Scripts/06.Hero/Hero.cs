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
        enemyLayer= LayerMask.GetMask("Monster");

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

}
