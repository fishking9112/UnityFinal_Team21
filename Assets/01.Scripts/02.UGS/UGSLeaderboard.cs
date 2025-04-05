using System;
using System.Threading.Tasks;
using Unity.Services.Leaderboards;
using UnityEngine;

public class UGSLeaderboard : MonoBehaviour
{
    private const string LeaderboardId = "TestBoard"; // Dashboard에서 설정한 ID


    /// <summary>
    /// 플레이어 점수 업로드
    /// </summary>
    /// <param name="score">점수</param>
    public async Task UploadScoreAsync(int score)
    {
        try
        {
            var response = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, score);
            Debug.Log($"점수 업로드: {response.Score}");
        }
        catch (Exception e)
        {
            Debug.LogError($"점수 업로드 실패: {e.Message}");
        }
    }

    /// <summary>
    /// top10 순위 가져오기
    /// </summary>
    public async Task GetTop10ScoresAsync()
    {
        try
        {
            var scores = await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions
            {
                Limit = 10
            });

            foreach (var entry in scores.Results)
            {
                Debug.Log($"{entry.Rank}. {entry.PlayerName} : {entry.Score}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"리더보드 불러오기 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 플레이어 점수 가져오기
    /// </summary>
    public async Task GetMyRankAsync()
    {
        try
        {
            var entry = await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);
            Debug.Log($"내 순위: {entry.Rank}, 점수: {entry.Score}");
        }
        catch (Exception e)
        {
            Debug.LogWarning("랭킹 없음 또는 오류: " + e.Message);
        }
    }
}
