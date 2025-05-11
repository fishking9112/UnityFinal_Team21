using System.Collections.Generic;
using UnityEngine;

public class QueenActiveSkillManager : MonoBehaviour
{
    public GameObject allSkill;
    public Dictionary<int, QueenActiveSkillBase> queenActiveSkillDic;

    private void Start()
    {
        Init();

        // 테스트 코드
        Utils.DelayedTimeAction(() =>
        {
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(0, queenActiveSkillDic[(int)IDQueenActiveSkill.SUMMON]);
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(1, queenActiveSkillDic[(int)IDQueenActiveSkill.ATTACK_DAMAGE_UP]);
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(2, queenActiveSkillDic[(int)IDQueenActiveSkill.RANGE_HEAL]);
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(3, queenActiveSkillDic[(int)IDQueenActiveSkill.RANGE_SLOW]);
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(4, queenActiveSkillDic[(int)IDQueenActiveSkill.METEOR]);
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(5, queenActiveSkillDic[(int)IDQueenActiveSkill.ALL_HEAL]);
            GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(0, queenActiveSkillDic[(int)IDQueenActiveSkill.SACRIFICE]);
            GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(1, queenActiveSkillDic[(int)IDQueenActiveSkill.MANARECYCLE]);
            GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(2, queenActiveSkillDic[(int)IDQueenActiveSkill.OVERWORK]);
            GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(3, queenActiveSkillDic[(int)IDQueenActiveSkill.RECALL]);
        }, 3);
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
}