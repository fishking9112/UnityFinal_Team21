using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class QueenActiveSkillManager : MonoSingleton<QueenActiveSkillManager>
{
    public GameObject allSkill;
    public Dictionary<int, QueenActiveSkillBase> queenActiveSkillDic;
    private QueenActiveSkillSlot skillSlot;

    private async void Start()
    {
        await Init();
    }

    private async UniTask Init()
    {
        await UniTask.WaitUntil(() => GameManager.Instance.queen.controller.queenActiveSkillSlot != null);

        skillSlot = GameManager.Instance.queen.controller.queenActiveSkillSlot;

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

    public void AddSkill(int id)
    {
        skillSlot.AddSlotToEmpty(queenActiveSkillDic[id]);
    }

    public void AddSkill(int index, int id)
    {
        skillSlot.AddSlot(index, queenActiveSkillDic[id]);
    }

    public void RemoveSkill(int index)
    {
        skillSlot.RemoveSlot(index);
    }
}