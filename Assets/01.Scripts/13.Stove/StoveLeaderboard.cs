using Cysharp.Threading.Tasks;
using Stove.PCSDK.NET;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
public struct StoveRankInfo
{
    public uint Rank; // uint 유지
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

    /// <summary>
    /// 유저 닉네임 불러오기
    /// </summary>
    public async UniTask GetMyNicknameAsync()
    {
        var tcs = new UniTaskCompletionSource<bool>();

        StovePCCallback callback = new StovePCCallback
        {
            OnUser = (user) =>
            {
                myNickname = user.Nickname;
                Utils.Log($"내 닉네임: {myNickname}");
                tcs.TrySetResult(true);
            }
        };

        StovePC.Initialize(new StovePCConfig(), callback);
        StovePC.GetUser();
        await tcs.Task;
    }

    /// <summary>
    /// 점수 업로드
    /// </summary>
    public async UniTask UploadScoreAsync(int score)
    {
        Utils.Log($"점수 업로드 요청: {score}");
        StovePC.SetStat(RankStatId, score);
        await UniTask.Delay(500);
    }

    public async UniTask GetTop10ScoresAsync()
    {
        var tcs = new UniTaskCompletionSource<bool>();

        StovePCCallback callback = new StovePCCallback
        {
            OnRank = (ranks, totalCount) =>
            {
                rankerInfo.Clear();
                foreach (var rank in ranks)
                {
                    rankerInfo.Add(new StoveRankInfo(rank.Rank, rank.Nickname, 0, rank.Score));
                }

                Utils.Log("Top10 랭킹 조회 완료");
                tcs.TrySetResult(true);
            }
        };

        StovePC.Initialize(new StovePCConfig(), callback);
        StovePC.GetRank(LeaderboardId, 1, 10, false); // 내 순위 포함 X → 순수 Top10만 가져오기
        await tcs.Task;
    }

    public async UniTask GetMyRankAsync()
    {
        var tcs = new UniTaskCompletionSource<bool>();

        StovePCCallback callback = new StovePCCallback
        {
            OnRank = (ranks, totalCount) =>
            {
                myRankerInfo = default;

                foreach (var rank in ranks)
                {
                    if (rank.Nickname == myNickname)
                    {
                        myRankerInfo = new StoveRankInfo(rank.Rank, rank.Nickname, 0, rank.Score);
                        break;
                    }
                }

                Utils.Log("내 랭크 조회 완료");
                tcs.TrySetResult(true);
            }
        };

        StovePC.Initialize(new StovePCConfig(), callback);
        StovePC.GetRank(LeaderboardId, 1, 1, true); // 내 순위만 포함 → 최소 페이지로
        await tcs.Task;
    }
}
