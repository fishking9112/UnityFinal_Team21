using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class LogManager : MonoSingleton<LogManager>
{
    private Dictionary<int,Action<int>> eventDic = new Dictionary<int,Action<int>>();



    // Start is called before the first frame update
    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();

            AnalyticsService.Instance.StartDataCollection();

            eventDic = new Dictionary<int, Action<int>>()
            {
                {(int)GameLog.Contents.App,         AppEvent },
                {(int)GameLog.Contents.Account,     AccountEvent },
                {(int)GameLog.Contents.Play,        PlayEvent },
                {(int)GameLog.Contents.Character,   CharacterEvent },
                {(int)GameLog.Contents.Collection,  CollectionEvent},
                {(int)GameLog.Contents.Archieve,    ArchieveEvent},
                {(int)GameLog.Contents.QueenAbility,QueenAbilityEvent},
                {(int)GameLog.Contents.Leaderboard, LeaderBoardEvent},
                {(int)GameLog.Contents.Funnel,      FunnelEvent},
            };


            LogEvent(GameLog.Contents.Funnel, (int)GameLog.FunnelType.GameStart);
        }
        catch
        {

        }
    }

    public void LogEvent(GameLog.Contents contents,int type)
    {
        if(eventDic.TryGetValue((int)contents, out Action<int> action))
        {
            action(type);
        }
        else
        {
            Debug.LogError("없는 Type 사용");
        }
    }

    private void AppEvent(int type)
    {
        var funnelEvent = new CustomEvent("Funnel_Step"); 
        funnelEvent["Funnel_Step_Number"] = type; 

        AnalyticsService.Instance.RecordEvent(funnelEvent); 
    }
    private void AccountEvent(int type)
    {
        var funnelEvent = new CustomEvent("Funnel_Step"); 
        funnelEvent["Funnel_Step_Number"] = type; 

        AnalyticsService.Instance.RecordEvent(funnelEvent); 
    }
    private void PlayEvent(int type)
    {
        var funnelEvent = new CustomEvent("Funnel_Step"); 
        funnelEvent["Funnel_Step_Number"] = type; 

        AnalyticsService.Instance.RecordEvent(funnelEvent); 
    }
    private void CharacterEvent(int type)
    {
        var funnelEvent = new CustomEvent("Funnel_Step"); 
        funnelEvent["Funnel_Step_Number"] = type; 

        AnalyticsService.Instance.RecordEvent(funnelEvent); 
    }
    private void CollectionEvent(int type)
    {
        var funnelEvent = new CustomEvent("Funnel_Step"); 
        funnelEvent["Funnel_Step_Number"] = type; 

        AnalyticsService.Instance.RecordEvent(funnelEvent); 
    }
    private void ArchieveEvent(int type)
    {
        var funnelEvent = new CustomEvent("Funnel_Step"); 
        funnelEvent["Funnel_Step_Number"] = type; 

        AnalyticsService.Instance.RecordEvent(funnelEvent); 
    }
    private void QueenAbilityEvent(int type)
    {
        var funnelEvent = new CustomEvent("Funnel_Step"); 
        funnelEvent["Funnel_Step_Number"] = type; 

        AnalyticsService.Instance.RecordEvent(funnelEvent); 
    }
    private void LeaderBoardEvent(int type)
    {
        var funnelEvent = new CustomEvent("Funnel_Step"); 
        funnelEvent["Funnel_Step_Number"] = type; 

        AnalyticsService.Instance.RecordEvent(funnelEvent); 
    }


    private void FunnelEvent(int type)
    {
        var funnelEvent = new CustomEvent(GameLog.funnel); 
        funnelEvent[GameLog.funnel_step] = type; 

        AnalyticsService.Instance.RecordEvent(funnelEvent); 
    }


}
