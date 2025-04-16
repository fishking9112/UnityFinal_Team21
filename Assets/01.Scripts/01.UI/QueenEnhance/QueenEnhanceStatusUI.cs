using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class QueenEnhanceStatusUI : MonoBehaviour
{
    private QueenCondition queenCondition;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI enhanceText;

    private void Start()
    {
        queenCondition = GameManager.Instance.queen.condition;
    }

    public void RefreshStatus()
    {
        var builder = new StringBuilder();

        // 1. 기본 스탯 표시
        // 마나 (n / max)
        float curMana = queenCondition.CurMagicGauge.Value;
        float maxMana = queenCondition.MaxMagicGauge.Value;
        builder.AppendLine($"마나 : ({(int)curMana} / {(int)maxMana})");
        
        // 마나 회복량 (ex: n / sec)
        float manaRegen = queenCondition.magicGaugeRecoverySpeed;
        builder.AppendLine($"마나 회복량 : {manaRegen:F1} / sec");
        
        // 소환 게이지 (n / max)
        float curSummon = queenCondition.CurSummonGauge.Value;
        float maxSummon = queenCondition.MaxSummonGauge.Value;
        builder.AppendLine($"소환 게이지 : {(int)curSummon} / {(int)maxSummon})");
        
        // 소환 회복량
        float summonRegen = queenCondition.summonGaugeRecoverySpeed;
        builder.AppendLine($"소환 회복량 : {summonRegen:F1} / sec");
       
        // TODO : 해야함.
        int castleHp = 100;
        int maxCastleHp = 100;
        builder.AppendLine($"캐슬 체력 : ({castleHp}/{maxCastleHp})");

        builder.AppendLine("------------------------------------");

        // 2. 종족별 강화 효과 표시 (MonsterPassive만)
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
}
