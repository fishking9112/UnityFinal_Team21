using Cysharp.Threading.Tasks;
using Stove.PCSDK.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[DisallowMultipleComponent]
public class StoveManager : MonoBehaviour
{
    private static StoveManager instance;
    public static StoveManager Instance
    {
        get
        {
            return instance;
        }
    }


    [SerializeField]
    private string env;
    [SerializeField]
    private string appKey;
    [SerializeField]
    private string appSecret;
    [SerializeField]
    private string gameId;
    [SerializeField]
    private StovePCLogLevel logLevel;
    [SerializeField]
    private string logPath;
    private float RunCallbacksIntervalInSeconds = 1f;

    private Coroutine runcallbacksCoroutine;

    public StovePCUser User { get; private set; }
    public StovePCOwnership[] Ownerships { get; private set; }

    private UniTaskCompletionSource<List<StoveRankInfo>> rankTcs;
    private List<StoveRankInfo> latestRankList;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Debug.Log("Destroying StoveManager");
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(gameObject);

        Initialize();
    }

    private void OnDestroy()
    {
        if (runcallbacksCoroutine != null)
        {
            StopCoroutine(runcallbacksCoroutine);
            runcallbacksCoroutine = null;
        }

        StovePC.Uninitialize();
    }

    private IEnumerator RunCallbacks(float intervalSeconds)
    {
        var wfs = new WaitForSecondsRealtime(intervalSeconds);
        while (true)
        {
            StovePC.RunCallback();
            yield return wfs;
        }
    }

    private void WriteLog(string log)
    {
        Debug.Log(string.Concat(log, Environment.NewLine));
    }

    private void Initialize()
    {
        #region Log
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Initializing");
        sb.AppendFormat(" - nothing");
        WriteLog(sb.ToString());
        #endregion

        StovePCConfig config = new StovePCConfig
        {
            Env = this.env,
            AppKey = this.appKey,
            AppSecret = this.appSecret,
            GameId = this.gameId,
            LogLevel = this.logLevel,
            LogPath = this.logPath
        };

        StovePCCallback callback = new StovePCCallback
        {
            OnError = new StovePCErrorDelegate(this.OnError),
            OnInitializationComplete = new StovePCInitializationCompleteDelegate(this.OnInitializationComplete),
            OnUser = new StovePCUserDelegate(this.OnUser),
            OnOwnership = new StovePCOwnershipDelegate(this.OnOwnership),

            OnStat = new StovePCStatDelegate(this.OnStat),
            OnSetStat = new StovePCSetStatDelegate(this.OnSetStat),
            OnRank = new StovePCRankDelegate(this.OnRank),
        };

        StovePCResult callResult = StovePC.Initialize(config, callback);
        HandleStoveCallResult(StovePCFunctionType.Initialize, callResult);
    }

    private void OnError(StovePCError error)
    {
        #region Log
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("OnError");
        sb.AppendFormat(" - error.FunctionType : {0}" + Environment.NewLine, error.FunctionType.ToString());
        sb.AppendFormat(" - error.Result : {0}" + Environment.NewLine, (int)error.Result);
        sb.AppendFormat(" - error.Message : {0}" + Environment.NewLine, error.Message);
        sb.AppendFormat(" - error.ExternalError : {0}", error.ExternalError.ToString());
        WriteLog(sb.ToString());
        #endregion

        Debug.Log(error.FunctionType);

        switch (error.FunctionType)
        {
            case StovePCFunctionType.Initialize:
            case StovePCFunctionType.GetUser:
            case StovePCFunctionType.GetOwnership:
                BeginQuitAppDueToError();
                break;
        }
    }

    private void OnInitializationComplete()
    {
        #region Log
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("OnInitializationComplete");
        sb.AppendFormat(" - nothing");
        WriteLog(sb.ToString());
        #endregion

        if (this.User.MemberNo == 0)
        {
            StovePCResult callResult = StovePC.GetUser();
            HandleStoveCallResult(StovePCFunctionType.GetUser, callResult);
        }
    }

    private void OnUser(StovePCUser user)
    {
        #region Log
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("OnUser");
        sb.AppendFormat(" - user.MemberNo : {0}" + Environment.NewLine, user.MemberNo.ToString());
        sb.AppendFormat(" - user.Nickname : {0}" + Environment.NewLine, user.Nickname);
        WriteLog(sb.ToString());
        #endregion

        this.User = user;

        if (Ownerships == null)
        {
            StovePCResult callResult = StovePC.GetOwnership();
            HandleStoveCallResult(StovePCFunctionType.GetOwnership, callResult);
        }
    }

    private void OnOwnership(StovePCOwnership[] ownerships)
    {
        #region Log
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("OnOwnership");
        sb.AppendFormat(" - ownerships.Length : {0}" + Environment.NewLine, ownerships.Length);
        for (int i = 0; i < ownerships.Length; i++)
        {
            sb.AppendFormat(" - ownerships[{0}].GameId : {1}" + Environment.NewLine, i, ownerships[i].GameId);
            sb.AppendFormat(" - ownerships[{0}].GameCode : {1}" + Environment.NewLine, i, ownerships[i].GameCode);
            sb.AppendFormat(" - ownerships[{0}].OwnershipCode : {1}" + Environment.NewLine, i, ownerships[i].OwnershipCode);
            sb.AppendFormat(" - ownerships[{0}].PurchaseDate : {1}" + Environment.NewLine, i, ownerships[i].PurchaseDate);
            sb.AppendFormat(" - ownerships[{0}].MemberNo : {1}", i, ownerships[i].MemberNo.ToString());

            if (i < ownerships.Length - 1)
                sb.AppendFormat(Environment.NewLine);
        }
        WriteLog(sb.ToString());
        #endregion

        this.Ownerships = ownerships;

        bool owned = false;

        foreach (var ownership in ownerships)
        {
            // [OwnershipCode] 1: Ownership valid, 2: Ownership invalid(구매 취소한 경우)
            if (ownership.MemberNo != this.User.MemberNo || ownership.OwnershipCode != 1)
            {
                continue;
            }

            // [GameCode] 3: BASIC, 5: DLC
            if ((ownership.GameId == this.gameId && ownership.GameCode == 3) || (ownership.GameId == this.gameId && ownership.GameCode == 4) )
            {
                owned = true;
            }

            // Check for DLC
            if (ownership.GameId == "YOUR_DLC_ID" && ownership.GameCode == 5)
            {

            }
        }

        if (owned)
        {
            EnterGameWorld();
        }
        else
        {
            BeginQuitAppDueToOwnership();
        }
    }

    private void OnStat(StovePCStat stat)
    {
        // Print Stat Info
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("OnStat");
        sb.AppendFormat(" - stat.StatFullId.GameId : {0}" + Environment.NewLine, stat.StatFullId.GameId);
        sb.AppendFormat(" - stat.StatFullId.StatId : {0}" + Environment.NewLine, stat.StatFullId.StatId);
        sb.AppendFormat(" - stat.MemberNo : {0}" + Environment.NewLine, stat.MemberNo.ToString());
        sb.AppendFormat(" - stat.CurrentValue : {0}" + Environment.NewLine, stat.CurrentValue.ToString());
        sb.AppendFormat(" - stat.UpdatedAt : {0}", stat.UpdatedAt.ToString());

        WriteLog(sb.ToString());
    }

    private void OnSetStat(StovePCStatValue statValue)
    {
        // Print Stat Update Result Info
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("OnSetStat");
        sb.AppendFormat(" - statValue.CurrentValue : {0}" + Environment.NewLine, statValue.CurrentValue.ToString());
        sb.AppendFormat(" - statValue.Updated : {0}" + Environment.NewLine, statValue.Updated.ToString());
        sb.AppendFormat(" - statValue.ErrorMessage : {0}", statValue.ErrorMessage);

        WriteLog(sb.ToString());
    }
    private void OnRank(StovePCRank[] ranks, uint totalCount)
    {
        List<StoveRankInfo> parsedRanks = new();

        for (int i = 0; i < ranks.Length; i++)
        {
            var r = ranks[i];
            parsedRanks.Add(new StoveRankInfo
            {
                MemberNo = r.MemberNo,
                Score = r.Score,
                Rank = r.Rank,
                Nickname = r.Nickname,
                ProfileImage = r.ProfileImage
            });
        }

        latestRankList = parsedRanks;

        // 콜백 대기중인 곳에 완료 신호 보내기
        rankTcs?.TrySetResult(latestRankList);

        latestRankList.Clear();

        foreach (var r in ranks)
        {
            latestRankList.Add(new StoveRankInfo
            {
                MemberNo = r.MemberNo,
                Score = r.Score,
                Rank = r.Rank,
                Nickname = r.Nickname,
                ProfileImage = r.ProfileImage
            });
        }
    }

    private void HandleStoveCallResult(StovePCFunctionType type, StovePCResult result)
    {
        #region Log
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("HandleStoveCallResult");
        sb.AppendFormat(" - type : {0}" + Environment.NewLine, type.ToString());
        sb.AppendFormat(" - result : {0}", result.ToString());
        WriteLog(sb.ToString());
        #endregion

        if (result == StovePCResult.NoError)
        {
            if (type == StovePCFunctionType.Initialize)
            {
                runcallbacksCoroutine = StartCoroutine(RunCallbacks(RunCallbacksIntervalInSeconds));
            }
        }
        else
        {
            switch (type)
            {
                case StovePCFunctionType.Initialize:
                case StovePCFunctionType.GetUser:
                case StovePCFunctionType.GetOwnership:
                    BeginQuitAppDueToError();
                    break;
            }
        }
    }

    private void BeginQuitAppDueToError()
    {
        #region Log
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("BeginQuitAppDueToError");
        sb.AppendFormat(" - nothing");
        WriteLog(sb.ToString());
        #endregion

        QuitApplication();
    }

    private void BeginQuitAppDueToOwnership()
    {
        #region Log
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("BeginQuitAppDueToOwnership");
        sb.AppendFormat(" - nothing");
        WriteLog(sb.ToString());
        #endregion

        QuitApplication();
    }

    private void EnterGameWorld()
    {
        #region Log
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("EnterGameWorld");
        sb.AppendFormat(" - nothing");
        WriteLog(sb.ToString());
        #endregion

    }

    public void QuitApplication()
    {
        #region Log
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("QuitApplication");
        sb.AppendFormat(" - nothing");
        WriteLog(sb.ToString());
        #endregion

        Application.Quit();
    }

    public StovePCResult GetStat(string statId)
    {
        return StovePC.GetStat(statId);
    }

    public async UniTask<List<StoveRankInfo>> GetRankAsync(string leaderboardId, uint pageIndex, uint pageSize, bool includeMyRank)
    {
        rankTcs = new UniTaskCompletionSource<List<StoveRankInfo>>();

        StovePCResult result = StovePC.GetRank(leaderboardId, pageIndex, pageSize, includeMyRank);
        if (result != StovePCResult.NoError)
        {
            Utils.LogError($"STOVE GetRank 실패: {result}");
            return new List<StoveRankInfo>();
        }

        return await rankTcs.Task;
    }

    public StovePCResult SetStat(string statId, int value)
    {
        return StovePC.SetStat(statId, value);
    }
}
