using System;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class RewardBase : MonoBehaviour, IPoolable
{
    #region IPoolable
    private Action<Component> returnToPool;

    public void Init(Action<Component> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnSpawn()
    {
        isActive = true;
        isMagnet = false;
    }

    public void OnDespawn()
    {
        isActive = false;
        isMagnet = false;
        returnToPool?.Invoke(this);
    }
    #endregion 

    [Header("자석 효과")]
    [SerializeField] private float magnetRadius = 3f;
    [SerializeField] private float magnetPower = 5f;

    protected QueenCondition condition;
    public float rewardAmount;

    private bool isActive;
    private bool isMagnet;

    private void Start()
    {
        condition = GameManager.Instance.queen.condition;
    }

    private void Update()
    {
        CursorMagnet();
    }

    private void CursorMagnet()
    {
        if (!isActive)
        {
            return;
        }

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float distance = Vector2.Distance(transform.position, mousePos);
       
        if(!isMagnet && distance < magnetRadius)
        {
            isMagnet = true;
        }

        if (isMagnet)
        {
            Vector3 dir = (mousePos - transform.position).normalized;
            transform.position += dir * magnetPower * Time.deltaTime;
        }
    }

    protected abstract void GainReward();

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Cursor"))
        {
            GainReward();
            OnDespawn();
        }
    }
}
