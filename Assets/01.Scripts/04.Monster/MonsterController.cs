using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

/// <summary>
/// 외형은 Prefab으로 미리 등록해서 사용
/// </summary>
public class MonsterController : BaseController, IPoolable
{
    #region IPoolable
    private Action<Component> returnToPool;

    public void Init(Action<Component> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnSpawn() // GetObject 이후
    {

    }

    public void OnDespawn() // 실행하면 자동으로 반환
    {
        _takeDamagedRendererCts?.Cancel();
        _takeDamagedRendererCts?.Dispose();
        _takeDamagedRendererCts = null;
        returnToPool?.Invoke(this);
    }
    #endregion

    [Header("현재 데이터")]
    [NonSerialized] public MonsterInfo monsterInfo;
    [NonSerialized] public Transform pivot;
    [NonSerialized] public SPUM_Prefabs spum;

    [NonSerialized] public Transform target;
    [NonSerialized] public NavMeshAgent navMeshAgent;
    [NonSerialized] public MonsterStateMachine stateMachine;
    [NonSerialized] public Vector2 projectileSize = Vector2.zero;
    [NonSerialized] public List<SpriteRenderer> renderers;
    [NonSerialized] public Collider2D _collider;

    [Header("넉백 관련 데이터")]
    [NonSerialized] public Vector2 knockback = Vector2.zero;
    [NonSerialized] public float knockbackPower = 0f;
    [NonSerialized] public float knockbackDuration = 0f;

    [Header("넉백 관련 데이터")]
    private CancellationTokenSource _takeDamagedRendererCts;
    [SerializeField] private float takeDamagedRendererTimer = 0.5f;

    private SortingGroup group;
    private int sortingOffset = 0;

    private void Update()
    {
        stateMachine.Update();

        // 테스트 코드 주석 처리
        // if (Input.GetMouseButtonDown(1))
        // {
        //     Die();
        // }
    }
    private void FixedUpdate()
    {
        stateMachine.FixedUpdate();
        group.sortingOrder = Mathf.RoundToInt(transform.position.y * -100) + sortingOffset;
    }

    /// <summary>
    /// 최초 생성 시 한번만 실행(참조해서 수치 자동 수정)
    /// </summary>
    /// <param name="monsterInfo">참조 할 수치 데이터</param>
    public void StatInit(MonsterInfo monsterInfo, bool isHealthUI)
    {
        if (!StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().gameResultUI.resultDatas.ContainsKey(monsterInfo.id))
        {
            StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().gameResultUI.resultDatas[monsterInfo.id] = new GameResultUnitData { spawnCount = 0, allDamage = 0 };
        }
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().gameResultUI.resultDatas[monsterInfo.id].spawnCount++;

        if (this.monsterInfo == null)
        {
            this.monsterInfo = new MonsterInfo(monsterInfo);
        }
        else
        {
            this.monsterInfo.Copy(monsterInfo);
        }

        base.StatInit(this.monsterInfo, isHealthUI);

        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = true;
        if (!navMeshAgent.isOnNavMesh)
        {
            OnDespawn();
            return;
        }
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;

        if (_collider == null)
        {
            _collider = GetComponent<Collider2D>();
        }
        _collider.enabled = true;

        if (pivot == null)
            pivot = transform.GetChild(0);

        if (spum == null)
        {
            spum = pivot.GetChild(0).GetComponent<SPUM_Prefabs>();
            if (!spum.allListsHaveItemsExist())
            {
                spum.PopulateAnimationLists();
            }
            spum.OverrideControllerInit();
        }

        // projectileObject의 XY값 가져오기
        if (monsterInfo.projectile != "" && projectileSize == Vector2.zero)
        {
            var projectileObject = ObjectPoolManager.Instance.GetObject<MonsterProjectileObject>(monsterInfo.projectile, transform.position);
            projectileSize = projectileObject._boxCollider.size;
            projectileObject.OnDespawn();
        }

        if (stateMachine == null)
            stateMachine = new(this);

        if (group == null)
        {
            group = GetComponent<SortingGroup>();
        }

        if (renderers == null)
        {
            renderers = new();
            renderers = gameObject.GetComponentsInChildren<SpriteRenderer>(true).Where(r => r.gameObject.name != "Shadow").ToList();
        }

        // alpha 1로 초기화
        foreach (var renderer in renderers)
        {
            renderer.color = Color.white;
        }

        stateMachine.ChangeState(stateMachine.Tracking); // 할 일 찾기

        MonsterManager.Instance.monsters.Add(gameObject, this);
        MonsterManager.Instance.idByMonsters[this.monsterInfo.id].Add(this);
    }

    /// <summary>
    /// 데미지를 입었을 경우
    /// </summary>
    /// <param name="damage"></param>
    public override void TakeDamaged(float damage)
    {
        base.TakeDamaged(damage);
        StaticUIManager.Instance.damageLayer.ShowDamage(damage, transform.position + Vector3.up * 0.5f);
        TakeDamagedRenderer();
    }

    /// <summary>
    /// 넉백을 입음
    /// </summary>
    /// <param name="other">넉백이 들어온 방향</param>
    /// <param name="power">넉백 계수</param>
    /// <param name="duration">지속 시간</param>
    public void TakeKnockback(Transform other, float power, float duration = 0.5f)
    {
        if (power == 0f) return;
        knockbackDuration = duration;
        knockbackPower = power;
        knockback = -(other.position - transform.position).normalized * power;
        stateMachine.ChangeState(stateMachine.Konckback); // 넉백
    }


    // UniTask 실행 함수
    public void TakeDamagedRenderer()
    {
        _takeDamagedRendererCts?.Cancel();
        _takeDamagedRendererCts?.Dispose();
        // 새로운 CancellationTokenSource 만들기 (OnDisable용 토큰도 연동)
        _takeDamagedRendererCts = new CancellationTokenSource();

        // Task 시작
        TakeDamagedRendererTask(_takeDamagedRendererCts.Token).Forget();
    }

    // UniTask 본문
    private async UniTaskVoid TakeDamagedRendererTask(CancellationToken token)
    {
        foreach (var renderer in stateMachine.Controller.renderers)
        {
            renderer.color = Color.red;
        }

        await UniTask.Delay(TimeSpan.FromSeconds(takeDamagedRendererTimer), cancellationToken: token);

        if (stateMachine.Controller == null) return;

        foreach (var renderer in stateMachine.Controller.renderers)
        {
            renderer.color = Color.white;
        }
    }


    /// <summary>
    /// 사망했을 경우
    /// </summary>
    public override void Die()
    {
        base.Die();

        MonsterManager.Instance.monsters.Remove(gameObject);
        MonsterManager.Instance.idByMonsters[this.monsterInfo.id].Remove(this);

        stateMachine.ChangeState(stateMachine.Die); // 사망
        // OnDespawn();
    }


    /// <summary>
    /// 업그레이드
    /// </summary>
    /// <param name="amount"></param>
    public void UpgradeHealth(float amount)
    {
        statHandler.health.AddOrigin(amount);
        HealthStatUpdate();
    }

    public void UpgradeAttack(float amount)
    {
        statHandler.attack.AddOrigin(amount);
    }

    public void UpgradeAttackSpeed(float amount)
    {
        statHandler.attackSpeed.AddOrigin(amount);
    }

    public void UpgradeMoveSpeed(float amount)
    {
        statHandler.moveSpeed.AddOrigin(amount);
    }
}
