using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Profiling.FrameDataView;

public class RankPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nickNameText;
    [SerializeField] private Image queenIconImage;
    [SerializeField] private TextMeshProUGUI scoreText;

    public void SetRankPanel(RankInfo rankInfo)
    {
        // 닉네임 설정
        nickNameText.text = rankInfo.Nickname;

        if (rankInfo.Score == 0)
        {
            rankText.text = "-";
            scoreText.text = "-";
            queenIconImage.enabled = false;
        }
        else
        {
            int showRankText = rankInfo.Rank + 1;
            // 랭크 표시 (100위 초과는 "100~")
            rankText.text = showRankText > 100 ? "100~" : showRankText.ToString();

            // 여왕 아이콘 설정
            if (DataManager.Instance.queenStatusDic.TryGetValue(rankInfo.QueenID, out var queenData))
            {
                queenIconImage.enabled = true;
                queenIconImage.sprite = DataManager.Instance.iconAtlas.GetSprite(queenData.Icon);
            }
            else
            {
                queenIconImage.enabled = false; // 예외 처리
            }

            // 점수 표시 (콤마 포함)
            scoreText.text = Utils.GetThousandCommaText(rankInfo.Score);
        }
    }
}
