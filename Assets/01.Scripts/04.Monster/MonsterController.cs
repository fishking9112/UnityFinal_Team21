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
        // TODO : 죽을 때 크리스탈 반환?
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

    private SortingGroup group;
    private int sortingOffset = 0;

    private void Update()
    {
        stateMachine.Update();

        if (Input.GetMouseButtonDown(1))
        {
            Die();
            // monster.GetComponent<MonsterController>().fsm.Setup(testTarget);
        }
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
        this.monsterInfo = monsterInfo;
        base.StatInit(this.monsterInfo);

        if (navMeshAgent == null)
            navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        navMeshAgent.enabled = true;

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

        stateMachine.ChangeState(stateMachine.FindToDo); // 할 일 찾기

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
        MonsterManager.Instance.monsters.Remove(gameObject);
        MonsterManager.Instance.idByMonsters[this.monsterInfo.id].Remove(this);

        stateMachine.ChangeState(stateMachine.Die); // 사망
        // OnDespawn();
    }
}
