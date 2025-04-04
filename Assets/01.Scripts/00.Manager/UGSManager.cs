using UnityEngine;

public class UGSManager : MonoSingleton<UGSManager>
{
    public UGSAuth Auth { get; private set; }
    public UGSSaveLoad SaveLoad { get; private set; }
    public UGSLeaderboard Leaderboard { get; private set; }

    protected override void Awake()
    {
        base.Awake();

        Auth = GetComponentInChildren<UGSAuth>();
        SaveLoad = GetComponentInChildren<UGSSaveLoad>();
        Leaderboard = GetComponentInChildren<UGSLeaderboard>();

        if (Auth == null || SaveLoad == null || Leaderboard == null)
        {
            Debug.LogError("UGS 관련 스크립트 누락");
        }
    }

}
