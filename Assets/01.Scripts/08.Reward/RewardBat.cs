using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class RewardBat : MonoBehaviour, IPoolable
{
    #region IPoolable
    private Action<Component> returnToPool;
    public void Init(Action<Component> returnAction)
    {
        returnToPool = returnAction;
    }

    public void OnSpawn()
    {
        queenCondition = GameManager.Instance.queen.condition;
        castlePos = GameManager.Instance.castle.transform.position;
        transform.position = GameManager.Instance.castle.transform.position;
        token = new CancellationTokenSource();
        goldRewardImage.SetActive(false);
        expRewardImage.SetActive(false);
        rewardAmount = 0;
        SetTarget(token.Token).Forget();
    }

    public void OnDespawn()
    {
        token?.Cancel();
        token?.Dispose();

        if (curTarget != null)
        {
            curTarget.isBatTarget = false;
        }

        returnToPool?.Invoke(this);
    }
    #endregion

    private QueenCondition queenCondition;
    private CancellationTokenSource token;

    private Vector3 castlePos;
    private Vector3 targetPos;
    private RewardBase curTarget;
    public float moveSpeed = 5f;

    public GameObject goldRewardImage;
    public GameObject expRewardImage;

    private bool haveReward;
    private RewardType rewardType;
    private float rewardAmount;

    public bool isIdle;

    private void Update()
    {
        TargetMove();
        Flip();
        RewardProcess();
    }

    private async UniTaskVoid SetTarget(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                if (haveReward)
                {
                    targetPos = castlePos;
                    isIdle = false;
                }
                else
                {
                    if (curTarget == null || curTarget.isMagnet || !curTarget.gameObject.activeInHierarchy)
                    {
                        curTarget = FindNearReward();

                        if (curTarget != null)
                        {
                            targetPos = curTarget.transform.position;
                        }
                        else if (!isIdle)
                        {
                            Vector2 randomPos = UnityEngine.Random.insideUnitCircle * RewardManager.Instance.idleRange;
                            targetPos = new Vector3(castlePos.x + randomPos.x, castlePos.y + randomPos.y, castlePos.z);
                            isIdle = true;
                        }
                    }
                    else
                    {
                        targetPos = curTarget.transform.position;
                        isIdle = false;
                    }
                }

                await UniTask.Delay(100, cancellationToken: token);
            }
        }
        catch (OperationCanceledException)
        {
            // 중간에 취소 시 예외 처리. 무시해도 됨.
        }
    }

    private RewardBase FindNearReward()
    {
        if (RewardManager.Instance == null || RewardManager.Instance.rewards == null)
        {
            return null;
        }

        if (RewardManager.Instance.rewards.Count == 0)
        {
            return null;
        }

        RewardBase nearReward = null;
        float minDistance = float.MaxValue;
        Vector3 batPos = transform.position;

        foreach (var reward in RewardManager.Instance.rewards.Values)
        {
            if (reward == null)
            {
                continue;
            }

            if (reward.isMagnet)
            {
                continue;
            }
            if (reward.isBatTarget)
            {
                continue;
            }

            float distance = (reward.transform.position - batPos).sqrMagnitude;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearReward = reward;
            }
        }

        if (nearReward != null)
        {
            nearReward.isBatTarget = true;
        }

        return nearReward;
    }

    private void TargetMove()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    private void Flip()
    {
        Vector3 dir = (targetPos - transform.position).normalized;

        if (dir.sqrMagnitude > 0.01f)
        {
            if (dir.x > 0 && transform.localScale.x < 0 || dir.x < 0 && transform.localScale.x > 0)
            {
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
            }
        }
    }

    private void RewardProcess()
    {
        if (haveReward)
        {
            float distance = (transform.position - castlePos).sqrMagnitude;

            if (distance < 0.1f)
            {
                goldRewardImage.SetActive(false);
                expRewardImage.SetActive(false);
                haveReward = false;
                curTarget = null;

                switch (rewardType)
                {
                    case RewardType.GOLD:
                        queenCondition.AdjustGold(rewardAmount);
                        break;
                    case RewardType.EXP:
                        queenCondition.AdjustCurExpGauge(rewardAmount);
                        break;
                }
            }
        }
        else
        {
            RewardPickUp();
        }
    }

    private void RewardPickUp()
    {
        if (curTarget == null)
        {
            return;
        }
        if (curTarget.isMagnet)
        {
            return;
        }

        float distance = (transform.position - curTarget.transform.position).sqrMagnitude;

        if (distance < 0.01f)
        {
            haveReward = true;

            if (curTarget.type == RewardType.GOLD)
            {
                goldRewardImage.SetActive(true);
            }
            else if (curTarget.type == RewardType.EXP)
            {
                expRewardImage.SetActive(true);
            }

            rewardType = curTarget.type;
            rewardAmount = curTarget.rewardAmount;
            curTarget.isBatTarget = false;
            curTarget.OnDespawn();
            curTarget = null;

            targetPos = castlePos;
        }
    }

    private void OnDestroy()
    {
        token?.Cancel();
        token?.Dispose();
    }

    public void AdjustMoveSpeed(float amount)
    {
        moveSpeed = Math.Clamp(moveSpeed + amount, 0, float.MaxValue);
    }
}
