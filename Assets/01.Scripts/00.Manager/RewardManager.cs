using System;
using System.Collections.Generic;
using UnityEngine;

public class RewardManager : MonoSingleton<RewardManager>
{
    public Dictionary<GameObject, RewardBase> rewards = new Dictionary<GameObject, RewardBase>();

    public Transform goldTarget;
    public Transform expTarget;

    public int initBatCount = 3;
    public float initbatMoveSpeed = 5f;
    [NonSerialized] public float idleRange = 3f;
    public List<RewardBat> batList = new List<RewardBat>();

    private void Start()
    {
        for (int i = 0; i < initBatCount; i++)
        {
            RewardBat bat = SpawnRewardBat(initbatMoveSpeed);
            batList.Add(bat);
        }
    }

    public void SpawnReward(string key, Vector3 position, float amount)
    {
        RewardBase reward = ObjectPoolManager.Instance.GetObject<RewardBase>(key, position);
        reward.rewardAmount = amount;
    }

    public RewardBat SpawnRewardBat(float moveSpeed)
    {
        Vector3 castlePos = GameManager.Instance.castle.transform.position;
        Vector2 randomPos = UnityEngine.Random.insideUnitCircle * idleRange;
        Vector3 spawnPos = new Vector3(castlePos.x + randomPos.x, castlePos.y + randomPos.y, castlePos.z);

        var bat = ObjectPoolManager.Instance.GetObject<RewardBat>("RewardBat", spawnPos);
        bat.moveSpeed = moveSpeed;
        bat.isIdle = true;

        return bat;
    }
}
