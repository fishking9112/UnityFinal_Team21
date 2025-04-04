using UnityEngine;

public class UGSManager : MonoSingleton<UGSManager>
{
    public UGSAuth Auth { get; private set; }
    public UGSSave Save { get; private set; }
    public UGSLeaderboard Leaderboard { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Auth = GetComponentInChildren<UGSAuth>();
        Save = GetComponentInChildren<UGSSave>();
        Leaderboard = GetComponentInChildren<UGSLeaderboard>();

        if (Auth == null || Save == null || Leaderboard == null)
        {
            Debug.LogError("UGS 관련 스크립트 누락");
        }
    }
}
