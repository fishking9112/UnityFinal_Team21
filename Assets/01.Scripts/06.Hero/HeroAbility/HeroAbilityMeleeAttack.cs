using Cysharp.Threading.Tasks;
using System.Threading;
using Unity.Mathematics;
using UnityEngine;

public class HeroAbilityMeleeAttack : HeroAbilitySystem
{
    private Hero hero;

    private Animator animator;

    private CancellationTokenSource token;
    

    protected override void Start()
    {
        heroAbilityInfo = DataManager.Instance.heroAbilityDic[101];
        base.Start();


        hero = transform.GetComponent<Hero>();

        animator = this.GetComponentInChildren<Animator>();
        token = new CancellationTokenSource();
        AddAbility();
    }

    protected override void ActionAbility()
    {
        target = hero.FindNearestTarget();

        SwingSword(token.Token).Forget();
    }


    private async UniTaskVoid SwingSword(CancellationToken tk)
    {
        float angle;

        if (target==null)
        {
            angle = 0;
        }
        else
        {
            angle = Mathf.Atan2(target.transform.position.y - hero.transform.position.y,
                target.transform.position.x - hero.transform.position.x) * Mathf.Rad2Deg;
        }
        animator.SetBool("2_Attack", true);
        await UniTask.WaitUntil(()=> animator.GetCurrentAnimatorStateInfo(0).normalizedTime>=0.5f,cancellationToken: tk);

        // 충돌처리
        OverlapCheck(angle);

        await UniTask.WaitUntil(()=> animator.GetCurrentAnimatorStateInfo(0).normalizedTime>=1f,cancellationToken: tk);

    }

    public override void AbilityLevelUp()
    {
        base.AbilityLevelUp();
    }

    private void OverlapCheck(float angle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        Vector2 local= (Vector2)transform.position+ new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

        Collider2D[] col = Physics2D.OverlapCircleAll(local, size.x,1<<7);
        Utils.DrawOverlapCircle(local, 1, Color.red);
        foreach(var c in col)
        {
            if (MonsterManager.Instance.monsters.TryGetValue(c.gameObject, out var monster))
            {
                monster.TakeDamaged(damage);
                Utils.Log("맞음");
            }
        }
    }

    public override void DespawnAbility()
    {
        animator.SetBool("4_Death", true);
        token?.Cancel();
        token?.Dispose();
    }
    public override void SetAbilityLevel(int level)
    {
        base.SetAbilityLevel(level);
    }
}