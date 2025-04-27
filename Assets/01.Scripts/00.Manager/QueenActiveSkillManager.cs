using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class QueenActiveSkillManager : MonoBehaviour
{
    public GameObject allSkill;
    public Dictionary<int, QueenActiveSkillBase> queenActiveSkillDic;

    private void Start()
    {
        Init();

        // 테스트 코드
        TestCode().Forget();
    }

    private void Init()
    {
        queenActiveSkillDic = new Dictionary<int, QueenActiveSkillBase>();

        QueenActiveSkillBase[] skills = allSkill.GetComponents<QueenActiveSkillBase>();

        foreach (QueenActiveSkillBase skill in skills)
        {
            skill.Init();

            if (!queenActiveSkillDic.TryGetValue(skill.info.id, out var exist))
            {
                queenActiveSkillDic[skill.info.id] = skill;
            }
        }
    }

    private async UniTaskVoid TestCode()
    {
        // await UniTask.WaitUntil(() => GameManager.Instance.queen.controller.queenActiveSkillSlot != null);
        await UniTask.Delay(3000);
        GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(0, queenActiveSkillDic[12]);
        GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(1, queenActiveSkillDic[14]);
    }
}