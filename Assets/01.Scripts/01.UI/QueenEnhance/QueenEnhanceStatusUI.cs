using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class QueenEnhanceStatusUI : MonoBehaviour
{
    [SerializeField] private QueenCondition queenCondition;
    [SerializeField] private TextMeshProUGUI statusText;

    public void SetQueenCondition(QueenCondition queenCondition)
    {
        this.queenCondition = queenCondition;
    }

    public void RefreshStatus()
    {
        var builder = new StringBuilder();

        // 마나
        float curMana = queenCondition.CurMagicGauge.Value;
        float maxMana = queenCondition.MaxMagicGauge.Value;
        builder.AppendLine($"마나 : ({(int)curMana} / {(int)maxMana})");

        // 마나 회복량 = 기본 회복량 + 강화 효과
        float manaRegenBase = queenCondition.magicGaugeRecoverySpeed;
        float manaRegenEnhance = QueenEnhanceManager.Instance.GetEnhanceValueByID(1002);
        builder.AppendLine($"마나 회복량 : {FormatNumber(manaRegenBase)} + {FormatNumber(manaRegenEnhance)} / sec");

        // 소환 게이지 = 현재 + 강화 / 최대
        float maxSummonBase = queenCondition.MaxSummonGauge.Value;
        float maxSummonEnhance = QueenEnhanceManager.Instance.GetEnhanceValueByID(1003);
        float maxSummonGauge = maxSummonBase + maxSummonEnhance;
        builder.AppendLine($"소환 게이지 : {FormatNumber(maxSummonBase)} + {FormatNumber(maxSummonEnhance)} / {FormatNumber(maxSummonGauge)}");

        // 소환 회복량 = 기본 + 강화
        float summonRegenBase = queenCondition.summonGaugeRecoverySpeed;
        float summonRegenEnhance = QueenEnhanceManager.Instance.GetEnhanceValueByID(-1);
        builder.AppendLine($"소환 회복량 : {FormatNumber(summonRegenBase)} + {FormatNumber(summonRegenEnhance)} / sec");

        // 캐슬 체력 = 기본 + 강화 / 최대
        float castleHpBase = 100;
        float castleHpEnhance = 100;
        float maxCastleHp = castleHpBase + castleHpEnhance;
        builder.AppendLine($"캐슬 체력 : {FormatNumber(castleHpBase)} + {FormatNumber(castleHpEnhance)} / {FormatNumber(maxCastleHp)}");

        builder.AppendLine("------------------------------------");


        // 종족별 강화 효과 표시 (MonsterPassive만)
        var acquiredEnhances = QueenEnhanceManager.Instance.AcquiredEnhanceLevels;

        var orderedBroods = new List<string>();
        foreach (var enhanceID in acquiredEnhances.Keys)
        {
            if (acquiredEnhances[enhanceID] <= 0) continue;

            var info = DataManager.Instance.queenEnhanceDic[enhanceID];
            if (info.type != QueenEnhanceType.MonsterPassive) continue;

            if (!orderedBroods.Contains(info.brood.ToString()))
            {
                orderedBroods.Add(info.brood.ToString());
            }
        }

        foreach (var brood in orderedBroods)
        {
            builder.AppendLine($"{brood}");

            foreach (var info in DataManager.Instance.queenEnhanceDic.Values)
            {
                if (info.brood.ToString() != brood || info.type != QueenEnhanceType.MonsterPassive) continue;

                int level = QueenEnhanceManager.Instance.GetEnhanceLevel(info.ID);
                if (level <= 0) continue; 

                int value = info.state_Base + info.state_LevelUp * Mathf.Max(0, level - 1);
                builder.AppendLine($"- {info.name} : Lv.{level} (+{value})");
            }
            builder.AppendLine("------------------------------------");
        }

        statusText.text = builder.ToString();
    }
    private string FormatNumber(float value)
    {
        return value % 1 == 0 ? ((int)value).ToString() : value.ToString("F1");
    }

}
