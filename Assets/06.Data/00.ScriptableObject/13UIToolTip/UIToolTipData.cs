using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UIToolTipInfo : IInfo
{
    public int id;
    public string name;
    public string description;

    public int ID => id;
    public string Name => name;
    public string Description => description;
    public string Icon => string.Empty;
}

[CreateAssetMenu(fileName = "UIToolTipData", menuName = "Scriptable Object/New UIToolTipData")]
public class UIToolTipData : SheetDataReaderBase
{
    public List<UIToolTipInfo> infoList = new List<UIToolTipInfo>();

    private UIToolTipInfo uiToolTipInfo;

    public override void UpdateStat(List<GSTU_Cell> list)
    {
        uiToolTipInfo = new UIToolTipInfo();

        foreach (var cell in list)
        {
            switch (cell.columnId)
            {
                case "id":
                    uiToolTipInfo.id = Utils.StringToInt(cell.value);
                    break;
                case "name":
                    uiToolTipInfo.name = cell.value;
                    break;
                case "description":
                    uiToolTipInfo.description = cell.value;
                    break;
            }
        }
        infoList.Add(uiToolTipInfo);
    }

    public override void ClearInfoList()
    {
        infoList.Clear();
    }
}
