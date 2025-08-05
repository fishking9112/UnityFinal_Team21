using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ToolTipInfo : IInfo
{
    public int id;
    public string name;
    public string description;
    public string image;
    public int nextId;

    public int ID => id;
    public string Name => name;
    public string Description => description;
    public string Icon => string.Empty;
}

[CreateAssetMenu(fileName = "ToolTipData", menuName = "Scriptable Object/New ToolTipData")]
public class ToolTipData : SheetDataReaderBase
{
    public List<ToolTipInfo> infoList = new List<ToolTipInfo>();

    private ToolTipInfo toolTipInfo;

    public override void UpdateStat(List<GSTU_Cell> list)
    {
        toolTipInfo = new ToolTipInfo();

        foreach (var cell in list)
        {
            switch (cell.columnId)
            {
                case "id":
                    toolTipInfo.id = Utils.StringToInt(cell.value);
                    break;
                case "name":
                    toolTipInfo.name = cell.value;
                    break;
                case "description":
                    toolTipInfo.description = cell.value;
                    break;
                case "image":
                    toolTipInfo.image = cell.value;
                    break;
                case "nextId":
                    toolTipInfo.nextId = Utils.StringToInt(cell.value);
                    break;
            }
        }
        infoList.Add(toolTipInfo);
    }

    public override void ClearInfoList()
    {
        infoList.Clear();
    }
}
