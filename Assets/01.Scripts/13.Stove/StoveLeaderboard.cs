using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class StoveLeaderboard : MonoBehaviour
{
    private const string RankStatId = "LEADERBOARD_ID";
    private const string LeaderboardId = "GM-22B9-68593725_IND|LEADERBOARD_ID";

    public List<RankInfo> rankerInfo { get; private set; } = new();
    public RankInfo myRankerInfo { get; private set; }

    /// <summary>
    /// 점수 업로드 (QueenID 포함)
    /// </summary>
    public async UniTask UploadScoreAsync(int score, int queenCharacterID)
    {
        int combinedScore = score * 100 + queenCharacterID;
        StoveManager.Instance.SetStat(RankStatId, combinedScore);

        await UniTask.Delay(500); // 콜백 처리 안정화
    }

    /// <summary>
    /// Top10 랭크 조회
    /// </summary>
    public async UniTask GetTop10ScoresAsync()
    {
        rankerInfo.Clear();

        var ranklist = await StoveManager.Instance.GetRankAsync(LeaderboardId, 1, 10, false);

        foreach (var rank in ranklist)
        {
            int combined = (int)rank.Score;
            int queenId = combined % 100;
            int actualScore = combined / 100;

            rankerInfo.Add(new RankInfo(rank.Rank, rank.Nickname, queenId, actualScore));
        }
    }

    /// <summary>
    /// 내 랭크 단독 조회
    /// </summary>
    public async UniTask GetMyRankAsync()
    {
        var ranklist = await StoveManager.Instance.GetRankAsync(LeaderboardId, 1, 1, true);

        if (ranklist == null || ranklist.Count == 0)
        {
            Utils.LogError("내 랭크 정보가 없습니다. 아직 점수를 업로드하지 않았거나, 서버에서 데이터를 불러오지 못했습니다.");
            return;
        }

        var my = ranklist[0]; // 문서 기준: 내 랭크는 항상 0번에 위치
        int combined = (int)my.Score;
        int queenId = combined % 100;
        int actualScore = combined / 100;

        myRankerInfo = new RankInfo(my.Rank, my.Nickname, queenId, actualScore);
        Utils.Log($"STOVE 내 랭크 조회 완료  랭킹: {my.Rank}, 점수: {actualScore}, 여왕ID: {queenId}");
    }

}
