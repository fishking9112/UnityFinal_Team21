using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterProjectileObject : MonoBehaviour, IPoolable
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
        returnToPool?.Invoke(gameObject.GetComponent<MonsterProjectileObject>());
    }
    #endregion

    public bool fxOnDestory = true;

    [HideInInspector] public MonsterController baseController;
    private float currentDuration;
    private Vector2 direction;
    private bool isReady;
    private Transform pivot;
    private Rigidbody2D _rigidbody;
    public BoxCollider2D _boxCollider;

    private float bulletSpeed = 5f;
    private float bulletSize = 1f;
    float finalAttackDamage => baseController.statHandler.attack.Value;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        pivot = transform.GetChild(0);
    }

    protected virtual void Update()
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
    public void Set(Vector2 direction, MonsterController baseController)
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
        if (baseController == null) return; // 방어코드

        if (baseController.obstacleLayer.value == (baseController.obstacleLayer.value | (1 << collision.gameObject.layer)))
        {
            DestroyProjectile(collision.ClosestPoint(transform.position) - direction * .2f, fxOnDestory);
        }
        else if (baseController.attackLayer.value == (baseController.attackLayer.value | (1 << collision.gameObject.layer)))
        {
            if (HeroManager.Instance.hero.ContainsKey(collision.gameObject))
            {
                HeroManager.Instance.hero[collision.gameObject].TakeDamaged(finalAttackDamage);
                var id = baseController.monsterInfo.id;
                StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().gameResultUI.resultDatas[id].allDamage += finalAttackDamage;
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
