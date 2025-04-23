using System.Collections.Generic;
using UnityEngine;

public class QueenActiveSkillManager : MonoBehaviour
{
    public GameObject allSkill;
    public Dictionary<int, QueenActiveSkillBase> queenActiveSkillDic;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        queenActiveSkillDic = new Dictionary<int, QueenActiveSkillBase>();

        QueenActiveSkillBase[] skills = allSkill.GetComponents<QueenActiveSkillBase>();

        foreach (QueenActiveSkillBase skill in skills)
        {
            if (!queenActiveSkillDic.TryGetValue(skill.id, out var exist))
            {
                queenActiveSkillDic[skill.id] = skill;
            }
        }

    }

    private void Start()
    {
        // 테스트 코드
        GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(0, queenActiveSkillDic[0]);
        GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(1, queenActiveSkillDic[1]);
    }
}
