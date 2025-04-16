using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileObject : MonoBehaviour, IPoolable
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
        returnToPool?.Invoke(gameObject.GetComponent<ProjectileObject>());
    }
    #endregion

    public bool fxOnDestory = true;

    [HideInInspector] public BaseController baseController;
    private float currentDuration;
    private Vector2 direction;
    private bool isReady;
    private Transform pivot;
    private Rigidbody2D _rigidbody;
    public BoxCollider2D _boxCollider;

    private float bulletSpeed = 5f;
    private float bulletSize = 1f;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        pivot = transform.GetChild(0);
    }

    private void Update()
    {
        if (!isReady)
        {
            return;
        }

        currentDuration += Time.deltaTime;

        if (currentDuration > 5f)
        {
            DestroyProjectile(transform.position, false);
        }

        _rigidbody.velocity = direction * bulletSpeed;
    }

    /// <summary>
    /// 투사체의 초기 세팅
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="baseController"></param>
    public void Set(Vector2 direction, BaseController baseController)
    {
        this.baseController = baseController;
        this.direction = direction;
        currentDuration = 0;
        transform.localScale = Vector3.one * bulletSize;

        transform.right = this.direction;

        if (this.direction.x < 0)
            pivot.localRotation = Quaternion.Euler(180, 0, 0);
        else
            pivot.localRotation = Quaternion.Euler(0, 0, 0);

        isReady = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (baseController.obstacleLayer.value == (baseController.obstacleLayer.value | (1 << collision.gameObject.layer)))
        {
            DestroyProjectile(collision.ClosestPoint(transform.position) - direction * .2f, fxOnDestory);
        }
        else if (baseController.attackLayer.value == (baseController.attackLayer.value | (1 << collision.gameObject.layer)))
        {
            //? LATE : 나중에 한곳에 몰아야 할 듯(Hero나 Monster나)
            BaseController target = MonsterManager.Instance.testTarget.GetComponent<BaseController>();
            if (target != null)
            {
                target.TakeDamaged(baseController.statData.attack);
                //? LATE : 넉백 적용 할 것
            }

            DestroyProjectile(collision.ClosestPoint(transform.position), fxOnDestory);
        }
    }

    /// <summary>
    /// 오브젝트 파괴 시 파티클 및 pool에 반환
    /// </summary>
    /// <param name="position"></param>
    /// <param name="createFx"></param>
    private void DestroyProjectile(Vector3 position, bool createFx)
    {
        if (createFx)
        {
            // TODO : 화살 충돌 후 터지는 파티클
            // ex) projectileManager.CreateImpactParticlesAtPostion(position, rangeWeaponHandler);
        }

        isReady = false;
        OnDespawn();
    }
}
