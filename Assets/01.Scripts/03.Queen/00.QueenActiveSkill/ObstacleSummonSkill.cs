using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class ObstacleSummonSkill : QueenActiveSkillBase
{
    [Header("장애물 프리팹")]
    [SerializeField] private GameObject obstaclePrefab;

    private Vector3 castlePos;

    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.SUMMON_OBSTACLE];
        castlePos = GameManager.Instance.castle.transform.position;
    }

    public override void UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;

        Vector3 dir = (mousePos - castlePos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        GameObject obstacle = Instantiate(obstaclePrefab, mousePos, rotation);

        EndSKill(obstacle, info.value).Forget();
    }

    private async UniTaskVoid EndSKill(GameObject obstacle, float delay)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: obstacle.GetCancellationTokenOnDestroy());

            if (obstacle != null)
            {
                Destroy(obstacle);
            }
        }
        catch (OperationCanceledException)
        {
            
        }
    }
}
