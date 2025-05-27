using UnityEngine;
using System;
using System.Collections.Generic;
using GoogleSheetsToUnity;

public enum EventTableType
{
    NULL,
    Type_1,
    Type_2,
    Type_3,
    Type_4,
}

[Serializable]
public class EventInfo : IInfo
{
    public int id;
    public string name;
    public string description;
    public string icon;
    public EventTableType type;
    public int rank;
    public int count;
    public int reward;
    public float createHp;
    public float timer;
    public float spawnDuration;

    public int ID => id;
    public string Name => name;
    public string Description => description;
    public string Icon => icon;
}

[CreateAssetMenu(fileName = "EventData", menuName = "Scriptable Object/New EventData")]
public class EventData : SheetDataReaderBase
{
    public List<EventInfo> infoList = new List<EventInfo>();

    private EventInfo eventInfo;

    public override void UpdateStat(List<GSTU_Cell> list)
    {
        eventInfo = new EventInfo();

        foreach (var cell in list)
        {
            switch (cell.columnId)
            {
                case "id":
                    eventInfo.id = Utils.StringToInt(cell.value);
                    break;
                case "name":
                    eventInfo.name = cell.value;
                    break;
                case "description":
                    eventInfo.description = cell.value;
                    break;
                case "icon":
                    eventInfo.icon = cell.value;
                    break;
                case "type":
                    eventInfo.type = Utils.StringToEnum<EventTableType>(cell.value, EventTableType.NULL);
                    break;
                case "rank":
                    eventInfo.rank = Utils.StringToInt(cell.value);
                    break;
                case "count":
                    eventInfo.count = Utils.StringToInt(cell.value);
                    break;
                case "reward":
                    eventInfo.reward = Utils.StringToInt(cell.value);
                    break;
                case "createHp":
                    eventInfo.createHp = Utils.StringToFloat(cell.value);
                    break;
                case "timer":
                    eventInfo.timer = Utils.StringToFloat(cell.value);
                    break;
                case "spawnDuration":
                    eventInfo.spawnDuration = Utils.StringToFloat(cell.value);
                    break;
            }
        }
        infoList.Add(eventInfo);
    }

    public override void ClearInfoList()
    {
        infoList.Clear();
    }
}
