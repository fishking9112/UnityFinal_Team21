using Cysharp.Threading.Tasks;
using Stove.PCSDK.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveLeaderboard : MonoBehaviour
{
    public async UniTask UploadScoreAsync(int score)
    {
        Utils.Log($"점수 업로드 요청: {score}");
        StovePC.SetStat("LEADERBOARD_ID", score);
        await UniTask.Delay(500);
    }

    public async UniTask GetTop10ScoresAsync()
    {
        Utils.Log("리더보드 Top10 요청");
        StovePC.GetRank("LEADERBOARD_ID", 1, 10, true);
        await UniTask.Delay(500);
    }

    public async UniTask GetMyRankAsync()
    {
        Utils.Log("내 순위 요청");
        StovePC.GetRank("LEADERBOARD_ID", 1, 10, true);
        await UniTask.Delay(500);
    }
}
