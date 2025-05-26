using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;

public class QueenPassiveSkillManager : MonoSingleton<QueenPassiveSkillManager>
{
    public Dictionary<int, QueenPassiveSkillInfo> queenPassiveSkillDic;

    private async void Start()
    {
        await Init();
    }

    private async UniTask Init()
    {
        await UniTask.WaitUntil(() => DataManager.Instance.queenPassiveSkillDic != null);

        queenPassiveSkillDic = DataManager.Instance.queenPassiveSkillDic;
    }
    public void AddSkill(int id)
    {
        float value = queenPassiveSkillDic[id].value;

        switch (id)
        {
            case (int)IDQueenPassiveSkill.CASTLE_HEALTH_UP:
                GameManager.Instance.castle.condition.AdjustMaxHealth(value);
                break;
            case (int)IDQueenPassiveSkill.MAX_MANA_UP:
                GameManager.Instance.queen.condition.AdjustMaxQueenActiveSkillGauge(value);
                break;
            case (int)IDQueenPassiveSkill.MANA_REGEN_UP:
                GameManager.Instance.queen.condition.AdjustQueenActiveSkillGaugeRecoverySpeed(value);
                break;
            case (int)IDQueenPassiveSkill.MAX_SUMMONGAUGE_UP:
                GameManager.Instance.queen.condition.AdjustMaxSummonGauge(value);
                break;
            case (int)IDQueenPassiveSkill.SUMMONGAUGE_REGEN_UP:
                GameManager.Instance.queen.condition.AdjustSummonGaugeRecoverySpeed(value);
                break;
            case (int)IDQueenPassiveSkill.SKELETON_ATTACK_UP:
                foreach (var monster in MonsterManager.Instance.monsterInfoList.Values)
                {
                    if (monster.monsterBrood == MonsterBrood.Skeleton)
                    {
                        monster.attack += monster.attack * 0.01f * value;
                    }
                }
                break;
            case (int)IDQueenPassiveSkill.ORC_HEALTH_UP:
                foreach (var monster in MonsterManager.Instance.monsterInfoList.Values)
                {
                    if (monster.monsterBrood == MonsterBrood.Orc)
                    {
                        monster.health += monster.health * 0.01f * value;
                    }
                }
                break;
        }
    }
}
