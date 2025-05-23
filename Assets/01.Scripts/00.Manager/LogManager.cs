using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class LogManager : MonoSingleton<LogManager>
{
    private Dictionary<int,Action<int,int>> eventDic = new Dictionary<int,Action<int,int>>();



    // Start is called before the first frame update
    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();

            AnalyticsService.Instance.StartDataCollection();

            eventDic = new Dictionary<int, Action<int,int>>()
            {
                {(int)GameLog.Contents.Account, AccountEvent },
                {(int)GameLog.Contents.Play,    InGameEvent },
                {(int)GameLog.Contents.Lobby,   LobbyEvent },
                {(int)GameLog.Contents.Funnel,      FunnelEvent},
                //{(int)GameLog.Contents.App,         AppEvent },
                //{(int)GameLog.Contents.Account,     AccountEvent },
                //{(int)GameLog.Contents.Play,        PlayEvent },
                //{(int)GameLog.Contents.Character,   CharacterEvent },
                //{(int)GameLog.Contents.Collection,  CollectionEvent},
                //{(int)GameLog.Contents.Archieve,    ArchieveEvent},
                //{(int)GameLog.Contents.QueenAbility,QueenAbilityEvent},
                //{(int)GameLog.Contents.Leaderboard, LeaderBoardEvent},

            };

        }
        catch
        {

        }
    }

    public void LogEvent(GameLog.Contents contents, int type, int id = 0)
    {
        if (eventDic.TryGetValue((int)contents, out Action<int, int> action))
        {
            action(type, id);
        }
        else
        {
            Debug.LogError("없는 Type 사용");
        }
    }


    public void PlayStartLog(int tryCount)
    {
        var playEvent = new CustomEvent(GameLog.InGame);
        playEvent[GameLog.logType] = (int)GameLog.LogType.GameStart;
        playEvent[GameLog.eventID] = tryCount;

        AnalyticsService.Instance.RecordEvent(playEvent);
    }

    public void PlayEndLog(int queenid, int time, int clear, int mostSummon_ID, int mostSummonCnt,
        int leastSummon_ID, int leastSummonCnt, int MVPid,int tryCount)
    {
        var playEvent = new CustomEvent(GameLog.EndGame);
        playEvent[GameLog.logType] = (int)GameLog.LogType.GameEnd;
        playEvent[GameLog.eventID] = queenid;
        playEvent[GameLog.time] = time;
        playEvent[GameLog.isClear] = clear;
        playEvent[GameLog.mostSummon_ID] = mostSummon_ID;
        playEvent[GameLog.mostSummonCnt] = mostSummonCnt;
        playEvent[GameLog.leastSummon_ID] = leastSummon_ID;
        playEvent[GameLog.leastSummonCnt] = leastSummonCnt;
        playEvent[GameLog.MVP_ID] = MVPid;
        playEvent[GameLog.tryCount] = tryCount;

        AnalyticsService.Instance.RecordEvent(playEvent);
    }
    private void FunnelEvent(int type, int id=0)
    {
        var funnelEvent = new CustomEvent(GameLog.funnel);
        funnelEvent[GameLog.funnel_step] = type;

        AnalyticsService.Instance.RecordEvent(funnelEvent);
    }
    #region event
    private void AccountEvent(int type, int id)
    {
        var accountEvent = new CustomEvent(GameLog.account); 
        accountEvent[GameLog.logType] = type;
        accountEvent[GameLog.eventID] = id;

        AnalyticsService.Instance.RecordEvent(accountEvent); 
    }
    private void LobbyEvent(int type, int id)
    {
        var LobbyEvent = new CustomEvent(GameLog.lobby); 
        LobbyEvent[GameLog.logType] = type;
        LobbyEvent[GameLog.eventID] = id;

        AnalyticsService.Instance.RecordEvent(LobbyEvent); 
    }
    private void InGameEvent(int type, int id)
    {
        var inGameEvent = new CustomEvent(GameLog.InGame); 
        inGameEvent[GameLog.logType] = type;
        inGameEvent[GameLog.eventID] = id;

        AnalyticsService.Instance.RecordEvent(inGameEvent); 
    }
    #endregion
    #region dummy
    private void AppEvent(int type, int id)
    {
        var funnelEvent = new CustomEvent("Funnel_Step");
        funnelEvent["Funnel_Step_Number"] = type;
        funnelEvent["ID"] = id;

        AnalyticsService.Instance.RecordEvent(funnelEvent);
    }
    private void CharacterEvent(int type, int id)
    {
        var funnelEvent = new CustomEvent("Funnel_Step"); 
        funnelEvent["Funnel_Step_Number"] = type;
        funnelEvent["ID"] = id;

        AnalyticsService.Instance.RecordEvent(funnelEvent); 
    }
    private void CollectionEvent(int type, int id)
    {
        var funnelEvent = new CustomEvent("Funnel_Step"); 
        funnelEvent["Funnel_Step_Number"] = type;
        funnelEvent["ID"] = id;

        AnalyticsService.Instance.RecordEvent(funnelEvent); 
    }
    private void ArchieveEvent(int type, int id)
    {
        var funnelEvent = new CustomEvent("Funnel_Step"); 
        funnelEvent["Funnel_Step_Number"] = type;
        funnelEvent["ID"] = id;

        AnalyticsService.Instance.RecordEvent(funnelEvent); 
    }
    private void QueenAbilityEvent(int type, int id)
    {
        var funnelEvent = new CustomEvent("Funnel_Step"); 
        funnelEvent["Funnel_Step_Number"] = type;
        funnelEvent["ID"] = id;

        AnalyticsService.Instance.RecordEvent(funnelEvent); 
    }
    private void LeaderBoardEvent(int type, int id)
    {
        var funnelEvent = new CustomEvent("Funnel_Step"); 
        funnelEvent["Funnel_Step_Number"] = type;
        funnelEvent["ID"] = id;

        AnalyticsService.Instance.RecordEvent(funnelEvent); 
    }
    #endregion

  


}
