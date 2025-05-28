using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardUI : MonoBehaviour
{
    [SerializeField] private Transform rankScrollContent;
    [SerializeField] private RankPanel myRankPanel;
    [SerializeField] private RankPanel rankPanelPrefab;
    [SerializeField] private GameObject noRankText;
    [SerializeField] private Button CloseBtn;

    private RankInfo myRankInfo;
    private List<RankInfo> rankerInfo;

    private void Awake()
    {
        noRankText.SetActive(false);
        CloseBtn.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void OnEnable()
    {
        rankerInfo = UGSManager.Instance.Leaderboard.rankerInfo;
        myRankInfo = UGSManager.Instance.Leaderboard.myRankerInfo;


        foreach (Transform child in rankScrollContent.transform)
        {
            Destroy(child.gameObject);
        }

        if(rankerInfo.Count <= 0)
        {
            noRankText.SetActive(true);
        }

        // 랭크는 10등까지만 표기
        for (int i = 0; i < rankerInfo.Count; i++)
        {
            RankPanel rankpanel = Instantiate(rankPanelPrefab, rankScrollContent);
            rankpanel.SetRankPanel(rankerInfo[i]);
        }

        myRankPanel.SetRankPanel(myRankInfo);
    }
}
