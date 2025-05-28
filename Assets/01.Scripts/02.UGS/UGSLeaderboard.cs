using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using UnityEngine;
using static UnityEditor.Profiling.FrameDataView;

// 플레이어들의 랭크 정보를 담을 구조체
public struct RankInfo
{
    public int Rank;
    public string Nickname;
    public int QueenID;
    public int Score;

    public RankInfo(int rank, string nickname, int queenId, int score)
    {
        Rank = rank;
        Nickname = nickname;
        QueenID = queenId;
        Score = score;
    }
}

public class UGSLeaderboard : MonoBehaviour
{
    private const string LeaderboardId = "KillCount"; // Dashboard에서 설정한 ID

    public List<RankInfo> rankerInfo { get; private set; }
    public RankInfo myRankerInfo { get; private set; }

    /// <summary>
    /// 플레이어 점수 업로드
    /// </summary>
    /// <param name="score">점수</param>
    public async UniTask UploadScoreAsync(int score)
    {
        try
        {
            var response = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, score);
            Utils.Log($"점수 업로드: {response.Score}");
        }
        catch (Exception e)
        {
            Utils.Log($"점수 업로드 실패: {e.Message}");
        }
    }

    /// <summary>
    /// top10 순위 가져오기
    /// </summary>
    public async UniTask GetTop10ScoresAsync()
    {
        var rankList = new List<RankInfo>();

        try
        {
            var scores = await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions
            {
                Limit = 10
            });

            foreach (var entry in scores.Results)
            {
                // nickname과 여왕ID 둘 다 받아오기
                var (nickname, queenID) = await UGSManager.Instance.SaveLoad.LoadPublicDataWithQueenId(entry.PlayerId);

                rankList.Add(new RankInfo(entry.Rank, nickname, queenID, (int)entry.Score));
            }
        }
        catch (Exception e)
        {
            Utils.Log($"리더보드 불러오기 실패: {e.Message}");
        }

        rankerInfo = rankList;
    }


    /// <summary>
    /// 플레이어 점수 가져오기
    /// </summary>
    public async UniTask GetMyRankAsync()
    {
        try
        {
            var entry = await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);

            // nickname과 여왕ID 둘 다 받아오기
            var (nickname, queenID) = await UGSManager.Instance.SaveLoad.LoadPublicDataWithQueenId(entry.PlayerId);
            myRankerInfo = new RankInfo(entry.Rank, nickname, queenID, (int)entry.Score);
        }
        catch (Exception e)
        {
            string nickname = await UGSManager.Instance.Auth.LoadPublicDataByPlayerId(UGSManager.Instance.PlayerId);
            myRankerInfo = new RankInfo(-1, nickname, 0, 0);
            Utils.Log("랭킹 없음 또는 오류: " + e.Message);
        }
    }
}
