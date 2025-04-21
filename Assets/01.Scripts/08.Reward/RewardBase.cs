using System;
using UnityEngine;

public abstract class RewardBase : MonoBehaviour,IPoolable
{
    #region IPoolable
    private Action<Component> returnToPool;

    public void Init(Action<Component> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnSpawn()
    {
        throw new NotImplementedException();

    }

    public void OnDespawn()
    {
        returnToPool?.Invoke(this);
    }
    #endregion 

    public float rewardAmount;

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
