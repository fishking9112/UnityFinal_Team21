using UnityEngine;

public class SummonSkill : QueenActiveSkillBase
{
    public override void Init()
    {
        base.Init();

        info = DataManager.Instance.queenActiveSkillDic[202];
    }

    public override void UseSkill()
    {
        Vector3 mousePos = controller.worldMousePos;

        for (int i = 0; i < info.value; i++)
        {
            float angle = (360f / info.value) * i;
            float rad = angle * Mathf.Deg2Rad;

            Vector3 pos = new Vector3(Mathf.Cos(rad) * info.size, Mathf.Sin(rad) * info.size, 0f) + mousePos;

            var skeleton = ObjectPoolManager.Instance.GetObject<MonsterController>("Skeleton_Normal", pos);
            skeleton.StatInit(DataManager.Instance.monsterDic[1201]);
        }
    }

}
