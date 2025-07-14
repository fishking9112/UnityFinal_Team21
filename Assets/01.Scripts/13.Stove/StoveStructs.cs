using Stove.PCSDK.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StoveStatInfo
{
    public string GameId;
    public string StatId;
    public ulong MemberNo;
    public long CurrentValue;
    public string UpdatedAt;

    public StoveStatInfo(string gameId, string statId, ulong memberNo, long currentValue, string updatedAt)
    {
        GameId = gameId;
        StatId = statId;
        MemberNo = memberNo;
        CurrentValue = currentValue;
        UpdatedAt = updatedAt;
    }
}

[System.Serializable]
public struct StoveRankInfo
{
    public ulong MemberNo;
    public long Score;
    public uint Rank;
    public string Nickname;
    public string ProfileImage;

    public StoveRankInfo(ulong memberNo, long score, uint rank, string nickname, string profileImage)
    {
        MemberNo = memberNo;
        Score = score;
        Rank = rank;
        Nickname = nickname;
        ProfileImage = profileImage;
    }
}

