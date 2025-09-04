using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class ObstacleSummonSkill : QueenActiveSkillBase
{
    [Header("장애물 프리팹")]
    [SerializeField] private GameObject obstaclePrefab;

    private GameObject obstacleGroup;
    private Vector3 castlePos;

    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[(int)IDQueenActiveSkill.SUMMON_OBSTACLE];
        castlePos = GameManager.Instance.castle.transform.position;
    }

    public override async UniTask UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;
        Vector3 dir = (mousePos - castlePos).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0f, 0f, angle);

        Vector3 right = new Vector3(-dir.y, dir.x).normalized;

        obstacleGroup = new GameObject("ObstacleGroup");
        obstacleGroup.transform.position = mousePos;
        obstacleGroup.transform.rotation = rotation;

        float spacing = obstaclePrefab.transform.localScale.x;

        for (int i = 0; i < info.size; i++)
        {
            Vector3 offset = right * (i - (info.size - 1) / 2f) * spacing;
            GameObject obstacle = Instantiate(obstaclePrefab, mousePos + offset, rotation, obstacleGroup.transform);
        }

        try
        {
            // 스킬 지속 시간 대기
            await UniTask.Delay(
                TimeSpan.FromSeconds(info.value),
                false,
                PlayerLoopTiming.Update,
                cancellationToken: obstacleGroup.GetCancellationTokenOnDestroy()
            );

            if (obstacleGroup != null)
            {
                Destroy(obstacleGroup);
            }
        }
        catch (OperationCanceledException)
        {
            // 스킬이 중간에 끊겼을 때 예외. 무시해도 됨
        }
    }

    protected override bool RangeCheck()
    {
        return true;
    }
}
