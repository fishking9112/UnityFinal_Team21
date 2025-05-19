using UnityEngine;

public class RewardManager : MonoSingleton<RewardManager>
{
    public Transform goldTarget;
    public Transform expTarget;

    private ObjectPoolManager objectPoolManager;

    protected override void Awake()
    {
        base.Awake();

        objectPoolManager = ObjectPoolManager.Instance;
    }

    public void SpawnReward(string key, Vector3 position, float amount)
    {
        RewardBase reward = objectPoolManager.GetObject<RewardBase>(key, position);
        reward.rewardAmount = amount;
    }
}
