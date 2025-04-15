using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HeroAttackState : HeroBaseState
{
    private GameObject enemy;
    private CancellationTokenSource token;

    public HeroAttackState(HeroState state) : base(state)
    {
    }

    public override void Enter()
    {
        base.Enter();
        token = new CancellationTokenSource();
        state.dir = GetEnemyDir();
        Move(token.Token).Forget();
    }

    private async UniTaskVoid Move(CancellationToken tk)
    {
        while (!token.IsCancellationRequested)
        {
            while (enemy != null && enemy.activeSelf)
            {
                Bounds selfBounds = GetBounds(state.hero.gameObject);
                Bounds targetBounds = GetBounds(enemy);
                float distance = GetMinDistanceBetweenBounds(selfBounds, targetBounds);

                if (distance<0.3f)
                {
                    break;
                }

                state.hero.transform.Translate(state.moveSpeed * Time.deltaTime * state.dir);

                await UniTask.Yield(tk, true);
            }
            GetEnemyDir();
            await UniTask.Yield(tk, true);
        }
    }


    public override void Exit()
    {
        base.Exit();
        token?.Cancel();
        token = null;
        enemy = null;
    }

    private Vector2 GetEnemyDir()
    {
        Collider2D col = Physics2D.OverlapCircle(state.hero.transform.position, 3);
        if (col == null)
        {
            state.ChangeState(state.moveState);
            return state.GetDir();
        }
        else
        {
            enemy = col.gameObject;
            state.dir = col.transform.position - state.hero.transform.position;
            return state.dir;
        }

    }

    private Bounds GetBounds(GameObject obj)
    {
        var renderer = obj.GetComponent<SpriteRenderer>();
        return renderer != null ? renderer.bounds : new Bounds(obj.transform.position, Vector3.zero);
    }
    private float GetMinDistanceBetweenBounds(Bounds a, Bounds b)
    {
        float dx = Mathf.Max(0, Mathf.Abs(a.center.x - b.center.x) - (a.extents.x + b.extents.x));
        float dy = Mathf.Max(0, Mathf.Abs(a.center.y - b.center.y) - (a.extents.y + b.extents.y));
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

}
