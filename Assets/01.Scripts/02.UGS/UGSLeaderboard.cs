using Cysharp.Threading.Tasks;
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
        try
        {
            var scores = await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions
            {
                Limit = 10
            });

            foreach (var entry in scores.Results)
            {
                string nickname = await UGSManager.Instance.Auth.LoadPublicDataByPlayerId(entry.PlayerId);
                Utils.Log($"{entry.Rank}. {nickname} : {entry.Score}");
            }
        }
        catch (Exception e)
        {
            Utils.Log($"리더보드 불러오기 실패: {e.Message}");
        }
    }

    /// <summary>
    /// 플레이어 점수 가져오기
    /// </summary>
    public async UniTask GetMyRankAsync()
    {
        try
        {
            var entry = await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);
            string nickname = await UGSManager.Instance.Auth.LoadPublicDataByPlayerId(entry.PlayerId);

            Utils.Log($"내 순위: {entry.Rank}, 점수: {entry.Score}, 닉네임: {nickname}");
        }
        catch (Exception e)
        {
            Utils.Log("랭킹 없음 또는 오류: " + e.Message);
        }
    }
}
