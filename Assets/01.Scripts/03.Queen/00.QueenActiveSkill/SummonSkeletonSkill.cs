using UnityEngine;

public class SummonSkeletonSkill : QueenActiveSkillBase
{
    [SerializeField] private float summonRadius = 3f;
    [SerializeField] private int summonCount = 30;

    public override void UseSkill()
    {
        if (controller == null)
        {
            controller = GameManager.Instance.queen.controller;
        }

        Vector3 center = controller.worldMousePos;

        for(int i = 0; i < summonCount; i++)
        {
            float angle = (360f / summonCount) * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 pos = new Vector3(Mathf.Cos(rad) * summonRadius, Mathf.Sin(rad) * summonRadius, 0f) + center;

            var skeleton = ObjectPoolManager.Instance.GetObject<MonsterController>("Skeleton_Normal", pos);
            skeleton.StatInit(DataManager.Instance.monsterDic[1201]);
        }
    }
}
