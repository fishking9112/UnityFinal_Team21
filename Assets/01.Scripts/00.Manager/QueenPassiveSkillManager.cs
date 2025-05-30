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
        float amount = 0;

        switch (id)
        {
            case (int)IDQueenPassiveSkill.CASTLE_HEALTH_UP:
                amount = GameManager.Instance.castle.condition.initMaxCastleHealth * value;
                GameManager.Instance.castle.condition.AdjustMaxHealth(amount);
                break;
            case (int)IDQueenPassiveSkill.MAX_MANA_UP:
                amount = DataManager.Instance.queenStatusDic[GameManager.Instance.QueenCharaterID].mana_Base * value;
                GameManager.Instance.queen.condition.AdjustMaxQueenActiveSkillGauge(amount);
                break;
            case (int)IDQueenPassiveSkill.MANA_REGEN_UP:
                amount = DataManager.Instance.queenStatusDic[GameManager.Instance.QueenCharaterID].mana_Recorvery * value;
                GameManager.Instance.queen.condition.AdjustQueenActiveSkillGaugeRecoverySpeed(amount);
                break;
            case (int)IDQueenPassiveSkill.MAX_SUMMONGAUGE_UP:
                amount = DataManager.Instance.queenStatusDic[GameManager.Instance.QueenCharaterID].summon_Base * value;
                GameManager.Instance.queen.condition.AdjustMaxSummonGauge(amount);
                break;
            case (int)IDQueenPassiveSkill.SUMMONGAUGE_REGEN_UP:
                amount = DataManager.Instance.queenStatusDic[GameManager.Instance.QueenCharaterID].summon_Recorvery * value;
                GameManager.Instance.queen.condition.AdjustSummonGaugeRecoverySpeed(amount);
                break;
            case (int)IDQueenPassiveSkill.SKELETON_ATTACK_UP:
                foreach (var monster in MonsterManager.Instance.monsterInfoList.Values)
                {
                    if (monster.monsterBrood == MonsterBrood.Skeleton)
                    {
                        amount = DataManager.Instance.monsterDic[monster.id].attack * value;
                        monster.attack += amount;
                    }
                }
                break;
            case (int)IDQueenPassiveSkill.ORC_HEALTH_UP:
                foreach (var monster in MonsterManager.Instance.monsterInfoList.Values)
                {
                    if (monster.monsterBrood == MonsterBrood.Orc)
                    {
                        amount = DataManager.Instance.monsterDic[monster.id].health * value;
                        monster.health += amount;
                    }
                }
                break;
        }
    }
}
