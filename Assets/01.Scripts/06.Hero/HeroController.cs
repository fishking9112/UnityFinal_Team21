using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class HeroController : BaseController
{
    [SerializeField] private HeroState stateMachine;

    public NavMeshAgent navMeshAgent;
    [SerializeField] private Hero hero;
    [SerializeField] private GameObject eventMark;
    [SerializeField] private SortingGroup group;
    private Vector3 lastPos = new();

    public Transform pivot;

    private int currentDir;
    private int lastDir;

    private bool isDead;

    private CancellationTokenSource token = new CancellationTokenSource();

    [SerializeField] public HeroStatusInfo statusInfo;

    private void Update()
    {
        if (transform.position != lastPos)
        {
            lastPos = transform.position;
            group.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);
        }
    }
    public void InitHero()
    {
        stateMachine = new HeroState(hero, this);
        navMeshAgent = GetComponent<NavMeshAgent>();
        stateMachine.navMeshAgent = navMeshAgent;
        pivot = transform.GetChild(2);
        stateMachine.animator = GetComponentInChildren<Animator>();


        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        CheckFlip(token.Token).Forget();
    }



    public void StatInit(HeroStatusInfo stat, bool isHealthUI, bool isEventMark = false)
    {
        hero.Init(stat.detectedRange);
        navMeshAgent.speed = stat.moveSpeed;

        isDead = false;
        base.StatInit(stat, isHealthUI);
        this.statusInfo.Copy(stat);

        hero.ResetAbility();

        eventMark.SetActive(isEventMark);

        for (int i = 0; i < stat.weapon.Length; i++)
        {
            hero.SetAbilityLevel(stat.weapon[i], stat.weaponLevel[i]);
        }

        if (!navMeshAgent.isOnNavMesh)
        {
            Debug.LogError($"{gameObject.name}은 NavMesh 위에 있지 않습니다!");
        }

        stateMachine.ChangeState(stateMachine.moveState);
        token = new CancellationTokenSource();

    }

    public override void TakeDamaged(float damage)
    {

        Vector2 randomOffset = new Vector2(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.3f, 0.3f));
        Vector3 worldPos = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
        StaticUIManager.Instance.damageLayer.ShowDamage(damage, worldPos + Vector3.up * 0.5f);
        base.TakeDamaged(damage);
    }

    private async UniTaskVoid CheckFlip(CancellationToken tk)
    {
        lastDir = 0;
        float x;
        while (!tk.IsCancellationRequested)
        {
            if (navMeshAgent == null)
            {
                return;
            }

            x = navMeshAgent.desiredVelocity.x;

            currentDir = MathF.Sign(x);

            if (currentDir == 0)
            {
                currentDir = lastDir;
            }
            else if (currentDir != lastDir)
            {
                pivot.localScale = new Vector3(-currentDir, 1, 1);
                lastDir = currentDir;
            }

            await UniTask.WaitForSeconds(0.5f);
        }
    }

    public void SetMove(bool isMove)
    {
        stateMachine.animator.SetBool("1_Move", isMove);
    }

    public void SetAttack(bool isAttack)
    {
        stateMachine.animator.SetBool("2_Attack", isAttack);
    }
    public void SetDead(bool isDead)
    {
        stateMachine.animator.SetBool("4_Death", isDead);
    }

    public async UniTask GetAnimFinish()
    {
        await UniTask.WaitUntil(() => stateMachine.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        HeroPoolManager.Instance.ReturnObject(this);
        isDead = false;

    }

    public override void Die()
    {
        if (isDead) return;
        isDead = true;

        base.Die();

        stateMachine.ChangeState(stateMachine.deadState);
        ResetObj();
    }

    public void ResetObj()
    {
        //token?.Cancel();
        //token?.Dispose();
        SetMove(false);
        SetAttack(false);
    }
}
