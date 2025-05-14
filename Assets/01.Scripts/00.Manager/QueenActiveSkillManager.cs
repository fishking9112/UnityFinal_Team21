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
            GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(0, queenActiveSkillDic[(int)IDQueenActiveSkill.SKELETON_LEGION]);
            GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(1, queenActiveSkillDic[(int)IDQueenActiveSkill.ATTACK_DAMAGE_UP]);
            GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(2, queenActiveSkillDic[(int)IDQueenActiveSkill.HEAL_WAVE]);
            GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(3, queenActiveSkillDic[(int)IDQueenActiveSkill.HEAL_RAIN]);
            GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(4, queenActiveSkillDic[(int)IDQueenActiveSkill.FIRE_EXPLOSION]);
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(5, queenActiveSkillDic[(int)IDQueenActiveSkill.HEAL_RAIN]);
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(5, queenActiveSkillDic[(int)IDQueenActiveSkill.SACRIFICE]);
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(5, queenActiveSkillDic[(int)IDQueenActiveSkill.MANA_RECYCLE]);
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(5, queenActiveSkillDic[(int)IDQueenActiveSkill.OVERWORK]);
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(5, queenActiveSkillDic[(int)IDQueenActiveSkill.RECALL]);
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(5, queenActiveSkillDic[(int)IDQueenActiveSkill.SKELETONLEGION]);
            // GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(5, queenActiveSkillDic[(int)IDQueenActiveSkill.DEATHSYMBOL]);
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(5, queenActiveSkillDic[(int)IDQueenActiveSkill.CASTLE_INVINCIBLE]);
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(5, queenActiveSkillDic[(int)IDQueenActiveSkill.SUMMON_MILITIA]);
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(5, queenActiveSkillDic[(int)IDQueenActiveSkill.WARCRY]);
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(5, queenActiveSkillDic[(int)IDQueenActiveSkill.BLOOD_ROAR]);
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(5, queenActiveSkillDic[(int)IDQueenActiveSkill.GIANT_FORM]);
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(5, queenActiveSkillDic[(int)IDQueenActiveSkill.DECAY]);
            //GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(5, queenActiveSkillDic[(int)IDQueenActiveSkill.SUMMON_OBSTACLE]);
            GameManager.Instance.queen.controller.queenActiveSkillSlot.AddSlot(5, queenActiveSkillDic[(int)IDQueenActiveSkill.LIGHTNING_STORM]);
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