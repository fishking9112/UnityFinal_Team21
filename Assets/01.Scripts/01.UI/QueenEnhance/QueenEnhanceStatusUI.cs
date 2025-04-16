using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class QueenEnhanceStatusUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI enhanceText;

    public void RefreshStatus()
    {
        var builder = new StringBuilder();

        // Dictionary에서 값들만 추출해 GroupBy
        var grouped = DataManager.Instance.queenEnhanceDic.Values
            .GroupBy(info => info.brood);

        foreach (var group in grouped)
        {
            var brood = group.Key;
            bool hasEnhanceInGroup = false;

            foreach (var info in group)
            {
                int level = QueenEnhanceManager.Instance.GetEnhanceLevel(info.ID);
                if (level <= 0) continue;

                if (!hasEnhanceInGroup)
                {
                    builder.AppendLine($"[브루드: {brood}]");
                    hasEnhanceInGroup = true;
                }

                int value = info.state_Base + info.state_LevelUp * (level - 1);
                builder.AppendLine($"- {info.name} : Lv.{level} (+{value})");
            }
        }

        if (builder.Length == 0)
        {
            builder.Append("선택한 강화가 없습니다.");
        }

        enhanceText.text = builder.ToString();
    }
}
