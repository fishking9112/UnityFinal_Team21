using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public void StatInit(MonsterInfo monsterInfo)
    {
        if (!StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().gameResultUI.resultDatas.ContainsKey(monsterInfo.id))
        {
            StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().gameResultUI.resultDatas[monsterInfo.id] = new GameResultUnitData { spawnCount = 0, allDamage = 0 };
        }
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().gameResultUI.resultDatas[monsterInfo.id].spawnCount++;

        if (this.monsterInfo == null)
        {
            this.monsterInfo = new MonsterInfo(monsterInfo);
            base.StatInit(this.monsterInfo);
        }
        else
        {
            this.monsterInfo.Copy(monsterInfo);
        }

        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = true;
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
            var projectileObject = ObjectPoolManager.Instance.GetObject<ProjectileObject>(monsterInfo.projectile, transform.position);
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
            renderers = gameObject.GetComponentsInChildren<SpriteRenderer>(true).ToList();
        }

        // alpha 1로 초기화
        foreach (var renderer in renderers)
        {
            Color color = renderer.color;
            color.a = 1f;
            renderer.color = color;
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
    }

    /// <summary>
    /// 사망했을 경우
    /// </summary>
    protected override void Die()
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
    public override void UpgradeHealth(float amount)
    {
        monsterInfo.health += amount;
        HealthStatUpdate();
    }

    public override void UpgradeAttack(float amount)
    {
        monsterInfo.attack += amount;
    }

    public override void UpgradeAttackSpeed(float amount)
    {
        monsterInfo.attackSpeed += amount;
    }

    public override void UpgradeMoveSpeed(float amount)
    {
        Utils.Log("이속 감소");
        monsterInfo.moveSpeed += amount;
        Utils.Log($"{monsterInfo.moveSpeed}");
    }
}
