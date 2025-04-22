using System;
using UnityEngine;

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

    }

    public void OnDespawn()
    {
        returnToPool?.Invoke(this);
    }
    #endregion 

    protected QueenCondition condition;
    public float rewardAmount;

    private void Start()
    {
        condition = GameManager.Instance.queen.condition;
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
