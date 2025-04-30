using UnityEngine;
using System;
using System.Collections.Generic;
using GoogleSheetsToUnity;

public enum TrophyType
{
    NULL,
    nonStack,
    Stack,
}

[Serializable]
public class TrophyInfo : IInfo
{
    public int id;
    public string name;
    public string description;
    public string icon;
    public TrophyType type;
    public int maxCount;
    public int unLockID;
    public int reward;

    public int ID => id;
    public string Name => name;
    public string Description => description;
    public string Icon => icon;
}

[CreateAssetMenu(fileName = "TrophyData", menuName = "Scriptable Object/New TrophyData")]
public class TrophyData : SheetDataReaderBase
{
    public List<TrophyInfo> infoList = new List<TrophyInfo>();

    private TrophyInfo trophyInfo;

    public override void UpdateStat(List<GSTU_Cell> list)
    {
        trophyInfo = new TrophyInfo();

        foreach (var cell in list)
        {
            switch (cell.columnId)
            {
                case "id":
                    trophyInfo.id = Utils.StringToInt(cell.value);
                    break;
                case "name":
                    trophyInfo.name = cell.value;
                    break;
                case "description":
                    trophyInfo.description = cell.value;
                    break;
                case "icon":
                    trophyInfo.icon = cell.value;
                    break;
                case "type":
                    trophyInfo.type = Utils.StringToEnum<TrophyType>(cell.value, TrophyType.NULL);
                    break;
                case "maxCount":
                    trophyInfo.maxCount = Utils.StringToInt(cell.value);
                    break;
                case "unLockID":
                    trophyInfo.unLockID = Utils.StringToInt(cell.value);
                    break;
                case "reward":
                    trophyInfo.reward = Utils.StringToInt(cell.value);
                    break;
            }
        }
        infoList.Add(trophyInfo);
    }

    public override void ClearInfoList()
    {
        infoList.Clear();
    }
}
