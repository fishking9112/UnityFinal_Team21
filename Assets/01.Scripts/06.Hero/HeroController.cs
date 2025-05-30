using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class HeroController : BaseController
{
    [SerializeField] private HeroState stateMachine;

    public Collider2D _collider;
    public NavMeshAgent navMeshAgent;
    [SerializeField] private Hero hero;
    [SerializeField] private GameObject eventMark;
    [SerializeField] private SortingGroup group;
    private Vector3 lastPos = new();

    public Transform pivot;

    private int currentDir;
    private int lastDir;

    private bool isDead;

    private CancellationTokenSource token = null;

    [SerializeField] public HeroStatusInfo statusInfo;


    [Header("빨간색 점등 관련 데이터")]
    [NonSerialized] public List<SpriteRenderer> renderers = new();
    private List<Color> originalColors = new(); // 원본 색상 저장용
    private CancellationTokenSource _takeDamagedRendererCts;
    [NonSerialized] private float takeDamagedRendererTimer = 0.5f;
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


        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

    }



    public void StatInit(HeroStatusInfo stat, bool isHealthUI, bool isEventMark = false)
    {
        stateMachine.animator = GetComponentInChildren<Animator>();

        pivot = transform.GetChild(2);
        hero.Init(stat.detectedRange);

        if (_collider == null)
        {
            _collider = GetComponent<Collider2D>();
        }
        _collider.enabled = true;

        navMeshAgent.enabled = true;
        navMeshAgent.speed = stat.moveSpeed;

        isDead = false;
        stateMachine.animator.SetBool("isDeath", false);
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
            Utils.LogError($"{gameObject.name}은 NavMesh 위에 있지 않습니다!");
        }

        stateMachine.ChangeState(stateMachine.moveState);

        token?.Cancel();
        token?.Dispose();
        token = new CancellationTokenSource();
        CheckFlip(token.Token).Forget();

        // IF문 탈출
        renderers.Clear();
        renderers = pivot.GetComponentsInChildren<SpriteRenderer>(true).Where(r => r.gameObject.name != "Shadow").ToList();
        originalColors.Clear();

        // 각 renderer의 현재 색상 저장
        foreach (var renderer in renderers)
        {
            originalColors.Add(renderer.color);
        }
    }

    public override void TakeDamaged(float damage)
    {

        Vector2 randomOffset = new Vector2(UnityEngine.Random.Range(-0.3f, 0.3f), UnityEngine.Random.Range(-0.3f, 0.3f));
        Vector3 worldPos = transform.position + new Vector3(randomOffset.x, randomOffset.y + 0.6f, 0f);
        StaticUIManager.Instance.damageLayer.ShowDamage(damage, worldPos + Vector3.up * 0.5f);
        TakeDamagedRenderer();

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
            else
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
        stateMachine.animator.SetBool("isDeath", isDead);
    }

    public async UniTask GetAnimFinish()
    {

        // await UniTask.WaitUntil(() => stateMachine.animator.GetCurrentAnimatorStateInfo(0).IsName("DEATH"));
        await UniTask.Delay(TimeSpan.FromSeconds(1f), false, PlayerLoopTiming.Update);
        // await UniTask.WaitUntil(() => stateMachine.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.95f);
        _takeDamagedRendererCts?.Cancel();
        _takeDamagedRendererCts?.Dispose();
        _takeDamagedRendererCts = null;
        token?.Cancel();
        token?.Dispose();
        token = null;
        HeroPoolManager.Instance.ReturnObject(this);
    }

    public override void Die()
    {
        if (isDead) return;
        isDead = true;

        base.Die();

        navMeshAgent.enabled = false;
        _collider.enabled = false;

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

    // UniTask 실행 함수
    public void TakeDamagedRenderer()
    {
        _takeDamagedRendererCts?.Cancel();
        _takeDamagedRendererCts?.Dispose();
        _takeDamagedRendererCts = new CancellationTokenSource();

        // Task 시작
        TakeDamagedRendererTask(_takeDamagedRendererCts.Token).Forget();
    }

    // UniTask 본문
    private async UniTaskVoid TakeDamagedRendererTask(CancellationToken token)
    {
        try
        {
            foreach (var renderer in renderers)
            {
                renderer.color = Color.red;
            }
            await UniTask.Delay(TimeSpan.FromSeconds(takeDamagedRendererTimer), cancellationToken: token);
            SetOriginColor();
        }
        finally
        {
            if (isDead)
            {
                SetOriginColor();
            }
        }
    }

    private void SetOriginColor()
    {
        // 저장한 색상으로 복원
        for (int i = 0; i < renderers.Count; i++)
        {
            if (i < originalColors.Count)
            {
                renderers[i].color = originalColors[i];
            }
        }
    }

    private void OnDestroy()
    {
        _takeDamagedRendererCts?.Cancel();
        _takeDamagedRendererCts?.Dispose();
        _takeDamagedRendererCts = null;
        token?.Cancel();
        token?.Dispose();
        token = null;
    }
}
