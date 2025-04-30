using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum QueenActiveSkillType
{
    NULL,
    ATTACK,
    SUMMON,
    BUFF,
}

[Serializable]
public class QueenActiveSkillInfo : IInfo
{
    public int id;
    public string name;
    public string description;
    public string icon;
    public QueenActiveSkillType type;
    public float cost;
    public float value;
    public float range;
    public float size;
    public LayerMask target;
    public int required_Level;
    public int buff_ID;
    public int buff_Level;

    public int ID => id;
    public string Name => name;
    public string Description => description;
    public string Icon => icon;
}

[CreateAssetMenu(fileName = "QueenActiveSkillData", menuName = "Scriptable Object/New QueenActiveSKillData")]
public class QueenActiveSkillData : SheetDataReaderBase
{
    public List<QueenActiveSkillInfo> infoList = new List<QueenActiveSkillInfo>();

    private QueenActiveSkillInfo queenActiveSkillInfo;

    public override void UpdateStat(List<GSTU_Cell> list)
    {
        queenActiveSkillInfo = new QueenActiveSkillInfo();

        foreach (var cell in list)
        {
            switch (cell.columnId)
            {
                case "id":
                    queenActiveSkillInfo.id = Utils.StringToInt(cell.value);
                    break;
                case "name":
                    queenActiveSkillInfo.name = cell.value;
                    break;
                case "description":
                    queenActiveSkillInfo.description = cell.value;
                    break;
                case "icon":
                    queenActiveSkillInfo.icon = cell.value;
                    break;
                case "type":
                    queenActiveSkillInfo.type = Utils.StringToEnum<QueenActiveSkillType>(cell.value, QueenActiveSkillType.NULL);
                    break;
                case "cost":
                    queenActiveSkillInfo.cost = Utils.StringToFloat(cell.value);
                    break;
                case "value":
                    queenActiveSkillInfo.value = Utils.StringToFloat(cell.value);
                    break;
                case "range":
                    queenActiveSkillInfo.range = Utils.StringToFloat(cell.value);
                    break;
                case "size":
                    queenActiveSkillInfo.size = Utils.StringToFloat(cell.value);
                    break;
                case "target":
                    queenActiveSkillInfo.target = LayerMask.GetMask(cell.value);
                    break;
                case "required_Level":
                    queenActiveSkillInfo.required_Level = Utils.StringToInt(cell.value);
                    break;
                case "buff_ID":
                    queenActiveSkillInfo.buff_ID = Utils.StringToInt(cell.value);
                    break;
                case "buff_Level":
                    queenActiveSkillInfo.buff_Level = Utils.StringToInt(cell.value);
                    break;
            }
        }
        infoList.Add(queenActiveSkillInfo);
    }

    public override void ClearInfoList()
    {
        infoList.Clear();
    }
}