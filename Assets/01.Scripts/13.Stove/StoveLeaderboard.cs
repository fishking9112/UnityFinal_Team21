using Cysharp.Threading.Tasks;
using Stove.PCSDK.NET;
using System.Collections.Generic;
using UnityEngine;

public struct StoveRankInfo
{
    public uint Rank; // uint 그대로 유지
    public string Nickname;
    public int QueenID;
    public int Score;

    public StoveRankInfo(uint rank, string nickname, int queenId, int score)
    {
        Rank = rank;
        Nickname = nickname;
        QueenID = queenId;
        Score = score;
    }
}

public class StoveLeaderboard : MonoBehaviour
{
    private const string RankStatId = "LEADERBOARD_ID";
    private const string LeaderboardId = "GM-22B9-68593725_IND|LEADERBOARD_ID";

    public List<StoveRankInfo> rankerInfo { get; private set; } = new();
    public StoveRankInfo myRankerInfo { get; private set; }

    private string myNickname;
    private bool isInitialized = false;

    private void EnsureInitialize()
    {
        if (!isInitialized)
        {
            StovePC.Initialize(new StovePCConfig(), new StovePCCallback
            {
                OnError = (error) => Utils.Log($"STOVE Error: {error.Message}")
            });
            isInitialized = true;
        }
    }

    /// <summary>
    /// 점수 업로드
    /// </summary>
    public async UniTask UploadScoreAsync(int score, int queenCharacterID)
    {
        EnsureInitialize();

        int combinedScore = score * 100 + queenCharacterID;

        StovePC.SetStat(RankStatId, combinedScore);
        await UniTask.Delay(500);

        Debug.Log($"STOVE 점수 업로드 완료: 점수 {score}, 여왕ID {queenCharacterID}, 업로드값 {combinedScore}");
    }

    /// <summary>
    /// Top10 랭크 가져오기
    /// </summary>
    public async UniTask GetTop10ScoresAsync()
    {
        EnsureInitialize();

        var tcs = new UniTaskCompletionSource<bool>();

        StovePC.Initialize(new StovePCConfig(), new StovePCCallback
        {
            OnRank = (ranks, totalCount) =>
            {
                rankerInfo.Clear();

                foreach (var rank in ranks)
                {
                    int combined = (int)rank.Score;
                    int queenId = combined % 100;
                    int actualScore = combined / 100;

                    rankerInfo.Add(new StoveRankInfo(rank.Rank, rank.Nickname, queenId, actualScore));
                }

                Utils.Log("Top10 랭킹 조회 완료");
                tcs.TrySetResult(true);
            }
        });

        StovePC.GetRank(LeaderboardId, 1, 10, false);
        await tcs.Task;
    }

    /// <summary>
    /// 내 점수 및 순위 가져오기
    /// </summary>
    public async UniTask GetMyRankAsync()
    {
        EnsureInitialize();

        var tcs = new UniTaskCompletionSource<bool>();

        StovePC.Initialize(new StovePCConfig(), new StovePCCallback
        {
            OnRank = (ranks, totalCount) =>
            {
                myRankerInfo = default;

                foreach (var rank in ranks)
                {
                    if (rank.Nickname == myNickname)
                    {
                        int combined = (int)rank.Score;
                        int queenId = combined % 100;
                        int actualScore = combined / 100;

                        myRankerInfo = new StoveRankInfo(rank.Rank, rank.Nickname, queenId, actualScore);
                        break;
                    }
                }

                Utils.Log("내 랭크 조회 완료");
                tcs.TrySetResult(true);
            }
        });

        StovePC.GetRank(LeaderboardId, 1, 1, true);
        await tcs.Task;
    }
}
