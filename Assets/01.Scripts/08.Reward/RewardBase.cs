using System;
using UnityEngine;

public abstract class RewardBase : MonoBehaviour, IPoolable
{
    #region IPoolable
    private Action<Component> returnToPool;

    public virtual void Init(Action<Component> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnSpawn()
    {
        isMagnet = false;
    }

    public void OnDespawn()
    {
        isMagnet = false;
        returnToPool?.Invoke(this);
    }
    #endregion 

    [Header("자석 효과")]
    [SerializeField] private Transform magnetTarget;
    [SerializeField] private float magnetRadius = 3f;
    [SerializeField] private float magnetAcceleration = 10f;

    private float curMagnetSpeed = 0f;

    protected QueenCondition condition;
    public float rewardAmount;

    private bool isMagnet;

    private void Start()
    {
        condition = GameManager.Instance.queen.condition;
    }

    private void Update()
    {
        TargetMagnet();
    }

    private void TargetMagnet()
    {
        if (magnetTarget == null)
        {
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float distance = Vector2.Distance(transform.position, mousePos);
       
        if(!isMagnet && distance < magnetRadius)
        {
            isMagnet = true;
            curMagnetSpeed = 0f;
        }

        if (isMagnet)
        {
            curMagnetSpeed += magnetAcceleration * Time.deltaTime;

            Vector3 dir = (magnetTarget.position - transform.position).normalized;
            transform.position += dir * curMagnetSpeed * Time.deltaTime;
        }
    }
    protected void SetMagnetTarget(Transform target)
    {
        magnetTarget = target;
    }

    protected abstract void GainReward();

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("RewardGain"))
        {
            GainReward();
            OnDespawn();
        }
    }
}
