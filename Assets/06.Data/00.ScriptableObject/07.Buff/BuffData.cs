using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BuffInfo : IInfo
{
    public int id;
    public string name;
    public string description;
    public string icon;
    public float tick;
    public float durationTime;
    public int lv_1;
    public int lv_2;
    public int lv_3;

    public int ID => id;
}

[CreateAssetMenu(fileName = "BuffData", menuName = "Scriptable Object/New BuffData")]
public class BuffData : SheetDataReaderBase
{
    public List<BuffInfo> infoList = new List<BuffInfo>();

    private BuffInfo buffInfo;

    public override void UpdateStat(List<GSTU_Cell> list)
    {
        buffInfo = new BuffInfo();

        foreach(var cell in list)
        {
            switch (cell.columnId)
            {
                case "id":
                    buffInfo.id = Utils.StringToInt(cell.value);
                    break;
                case "name":
                    buffInfo.name = cell.value;
                    break;
                case "description":
                    buffInfo.description = cell.value;
                    break;
                case "icon":
                    buffInfo.icon = cell.value;
                    break;
                case "tick":
                    buffInfo.tick = Utils.StringToInt(cell.value);
                    break;
                case "durationTime":
                    buffInfo.durationTime = Utils.StringToFloat(cell.value);
                    break;
                case "lv_1":
                    buffInfo.lv_1 = Utils.StringToInt(cell.value);
                    break;
                case "lv_2":
                    buffInfo.lv_2 = Utils.StringToInt(cell.value);
                    break;
                case "lv_3":
                    buffInfo.lv_3 = Utils.StringToInt(cell.value);
                    break;
            }
        }
        infoList.Add(buffInfo);
    }

    public override void ClearInfoList()
    {
        infoList.Clear();
    }
}
