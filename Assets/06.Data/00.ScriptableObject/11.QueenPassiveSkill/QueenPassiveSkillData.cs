using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QueenPassiveSkillInfo : IInfo
{
    public int id;
    public string name;
    public string description;
    public string icon;
    public float value;

    public int ID => id;
    public string Name => name;
    public string Description => description;
    public string Icon => icon;
}

[CreateAssetMenu(fileName = "QueenPassiveSkillData", menuName = "Scriptable Object/New QueenPassiveSkillData")]
public class QueenPassiveSkillData : SheetDataReaderBase
{
    public List<QueenPassiveSkillInfo> infoList = new List<QueenPassiveSkillInfo>();

    private QueenPassiveSkillInfo queenPassiveSkillInfo;

    public override void UpdateStat(List<GSTU_Cell> list)
    {
        queenPassiveSkillInfo = new QueenPassiveSkillInfo();

        foreach (var cell in list)
        {
            switch (cell.columnId)
            {
                case "id":
                    queenPassiveSkillInfo.id = Utils.StringToInt(cell.value);
                    break;
                case "name":
                    queenPassiveSkillInfo.name = cell.value;
                    break;
                case "description":
                    queenPassiveSkillInfo.description = cell.value;
                    break;
                case "icon":
                    queenPassiveSkillInfo.icon = cell.value;
                    break;
                case "value":
                    queenPassiveSkillInfo.value = Utils.StringToFloat(cell.value);
                    break;
            }
        }
        infoList.Add(queenPassiveSkillInfo);
    }

    public override void ClearInfoList()
    {
        infoList.Clear();
    }
}
