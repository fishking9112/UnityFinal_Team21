using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class HeroAbilityMeleeAttack : HeroAbilitySystem
{
    private Hero hero;

    private Animator animator;

    private LayerMask targetLayer;

    private CancellationTokenSource tk;

    public override void Initialize(int id)
    {
        base.Initialize(id);


    }

    private void Start()
    {
        hero = transform.GetComponent<Hero>();

        animator = this.GetComponentInChildren<Animator>();

        targetLayer = LayerMask.GetMask("Monster", "Castle");

    }
    private void OnEnable()
    {
        Initialize((int)IDHeroAbility.SWORD);
        tk = new CancellationTokenSource();
    }
    protected override void ActionAbility()
    {
        if (hero == null || token.IsCancellationRequested)
        {
            return;
        }

        target = hero.FindNearestTarget();
        SwingSword(tk.Token).Forget();
    }

    private async UniTaskVoid SwingSword(CancellationToken tk)
    {
        float angle;

        if (animator == null)
            return;

        if (target == null)
        {
            animator.SetBool("1_Move", true);
            animator.SetBool("2_Attack", false);
            return;
        }
        else
        {
            angle = Mathf.Atan2(target.transform.position.y - hero.transform.position.y,
                target.transform.position.x - hero.transform.position.x) * Mathf.Rad2Deg;

            animator.SetBool("1_Move", false);
            animator.SetBool("2_Attack", true);

            await UniTask.Delay(300, false, PlayerLoopTiming.Update, cancellationToken: this.GetCancellationTokenOnDestroy());
            SpawnSwingSwordParticle(angle);
        }

        await UniTask.WaitUntil(() => animator != null && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f, cancellationToken: tk);

        // 충돌처리
        OverlapCheck(angle);

        await UniTask.WaitUntil(() => animator != null && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f, cancellationToken: tk);

    }

    public void SpawnSwingSwordParticle(float angle)
    {
        if (target == null)
        {
            return;
        }

        Vector3 spawnPos = hero.transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle)) * 0.5f;

        ParticleObject particle = ParticleManager.Instance.SpawnParticle("HeroAbilityMeleeAttack", spawnPos, new Vector3(0.5f, 0.5f, 1f));
        var main = particle.particle.main;
        main.startRotation = angle - 90f;
    }

    public override void AbilityLevelUp()
    {
        base.AbilityLevelUp();
    }

    private void OverlapCheck(float angle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        Vector2 local = (Vector2)transform.position + new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

        Collider2D[] col = Physics2D.OverlapCircleAll(local, size.x, targetLayer);
        Utils.DrawOverlapCircle(local, 1, Color.red);
        foreach (var c in col)
        {
            if (MonsterManager.Instance.monsters.TryGetValue(c.gameObject, out var monster))
            {
                monster.TakeKnockback(this.transform, knockback);
                monster.TakeDamaged(damage);
            }
            else if (GameManager.Instance.castle.gameObject == c.gameObject)
            {
                GameManager.Instance.castle.TakeDamaged(damage);
            }
        }
    }

    public override void DespawnAbility()
    {
        if (animator == null) return;

        animator.SetBool("1_Move", false);
        animator.SetBool("2_Attack", false);

        animator.SetBool("4_Death", true);
        token?.Cancel();
        token?.Dispose();
        tk?.Cancel();
        tk?.Dispose();
    }
    public override void SetAbilityLevel(int level)
    {
        base.SetAbilityLevel(level);
        token = new CancellationTokenSource();

    }

}