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
    public string image;
    public float mana_Base;
    public float mana_LevelUp;
    public float summon_Base;
    public float summon_LevelUp;
    public float mana_Recorvery;
    public float summon_Recorvery;
    public float baseActiveSkill;
    public float basePassiveSkill_1;
    public float basePassiveSkill_2;
    public float basePassiveSkill_3;

    public int ID => id;
    public string Name => name;
    public string Description => description;
    public string Icon => icon;
    public string Image => image;
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
                    queenStatusInfo.image = cell.value;
                    break;
                case "mana_Base":
                    queenStatusInfo.mana_Base = Utils.StringToFloat(cell.value);
                    break;
                case "mana_LevelUp":
                    queenStatusInfo.mana_LevelUp = Utils.StringToFloat(cell.value);
                    break;
                case "summon_Base":
                    queenStatusInfo.summon_Base = Utils.StringToFloat(cell.value);
                    break;
                case "summon_LevelUp":
                    queenStatusInfo.summon_LevelUp = Utils.StringToFloat(cell.value);
                    break;
                case "mana_Recorvery":
                    queenStatusInfo.mana_Recorvery = Utils.StringToFloat(cell.value);
                    break;
                case "summon_Recorvery":
                    queenStatusInfo.summon_Recorvery = Utils.StringToFloat(cell.value);
                    break;
                case "baseActiveSkill":
                    queenStatusInfo.baseActiveSkill = Utils.StringToFloat(cell.value);
                    break;
                case "basePassiveSkill_1":
                    queenStatusInfo.basePassiveSkill_1 = Utils.StringToInt(cell.value);
                    break;
                case "basePassiveSkill_2":
                    queenStatusInfo.basePassiveSkill_2 = Utils.StringToInt(cell.value);
                    break;
                case "basePassiveSkill_3":
                    queenStatusInfo.basePassiveSkill_3 = Utils.StringToInt(cell.value);
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
