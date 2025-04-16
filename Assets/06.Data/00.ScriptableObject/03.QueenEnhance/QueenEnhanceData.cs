using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum QueenEnhanceType
{
    NULL,
    QueenPassive,
    MonsterPassive
}
public enum QueenEnhanceBrood
{
    NULL,
    None,
    Smile,
    Skeleton,
}

[Serializable]
public class QueenEnhanceInfo : IInfo
{
    public int id;
    public string name;
    public string description;
    public int icon;
    public int maxLevel;
    public int state_Base;
    public int state_LevelUp;
    public QueenEnhanceType type;
    public QueenEnhanceBrood brood;

    public int ID => id;
}

[CreateAssetMenu(fileName = "QueenEnhanceData", menuName = "Scriptable Object/New QueenEnhanceData")]
public class QueenEnhanceData : SheetDataReaderBase
{
    public List<QueenEnhanceInfo> infoList = new List<QueenEnhanceInfo>();

    private QueenEnhanceInfo queenEnhanceInfo;

    public override void UpdateStat(List<GSTU_Cell> list)
    {
        queenEnhanceInfo = new QueenEnhanceInfo();

        foreach (var cell in list)
        {
            switch (cell.columnId)
            {
                case "id":
                    queenEnhanceInfo.id = Utils.StringToInt(cell.value);
                    break;
                case "name":
                    queenEnhanceInfo.name = cell.value;
                    break;
                case "description":
                    queenEnhanceInfo.description = cell.value;
                    break;
                case "icon":
                    queenEnhanceInfo.icon = Utils.StringToInt(cell.value);
                    break;
                case "maxLevel":
                    queenEnhanceInfo.maxLevel = Utils.StringToInt(cell.value);
                    break;
                case "state_Base":
                    queenEnhanceInfo.state_Base = Utils.StringToInt(cell.value);
                    break;
                case "state_LevelUp":
                    queenEnhanceInfo.state_LevelUp = Utils.StringToInt(cell.value);
                    break;
                case "type":
                    queenEnhanceInfo.type = Utils.StringToEnum<QueenEnhanceType>(cell.value, QueenEnhanceType.NULL);
                    break;
                case "brood":
                    queenEnhanceInfo.brood = Utils.StringToEnum<QueenEnhanceBrood>(cell.value, QueenEnhanceBrood.NULL);
                    break;
            }
        }
        infoList.Add(queenEnhanceInfo);
    }

    public override void ClearInfoList()
    {
        infoList.Clear();
    }
}
