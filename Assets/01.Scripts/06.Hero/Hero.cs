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

    // Start is called before the first frame update
    void Start()
    {
        randomDelay = 3f;
        moveSpeed = 5;


        //MVP용 임시 기능
        ChangeDir().Forget();
        Move().Forget();
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

    }
}
