using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QueenStatusInfo : IInfo
{
    public int id;
    public string name;
    public string description;
    public string icon;
    public int maxLevel;
    public LevelInfo[] levelInfo;

    public int ID => id;
    public string Name => name;
    public string Description => description;
    public string Icon => icon;
}

[CreateAssetMenu(fileName = "QueenStatusData", menuName = "Scriptable Object/New QueenStatusData")]
public class QueenStatusData : SheetDataReaderBase
{
    public List<QueenStatusInfo> infoList = new List<QueenStatusInfo>();

    private QueenStatusInfo queenStatusInfo;

    public override void UpdateStat(List<GSTU_Cell> list)
    {
        queenStatusInfo = new QueenStatusInfo();

        foreach (var cell in list)
        {
            switch (cell.columnId)
            {
                case "id":
                    queenStatusInfo.id = Utils.StringToInt(cell.value);
                    break;
                case "name":
                    queenStatusInfo.name = cell.value;
                    break;
                case "description":
                    queenStatusInfo.description = cell.value;
                    break;
                case "icon":
                    queenStatusInfo.icon = cell.value;
                    break;
                case "image":
                    queenStatusInfo.icon = cell.value;
                    break;
                case "maxLevel":
                    queenStatusInfo.maxLevel = Utils.StringToInt(cell.value);
                    queenStatusInfo.levelInfo = new LevelInfo[queenStatusInfo.maxLevel];
                    for (int i = 0; i < queenStatusInfo.maxLevel; i++)
                    {
                        queenStatusInfo.levelInfo[i] = new LevelInfo();
                    }
                    break;
            }
        }
        infoList.Add(queenStatusInfo);
    }

    public override void ClearInfoList()
    {
        infoList.Clear();
    }
}
