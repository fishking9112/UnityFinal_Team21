using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum BuffType
{
    NULL,
    BUff,
    Debuff,
    Aura,
}

[Serializable]
public class BuffInfo : IInfo
{
    public int id;
    public string name;
    public string description;
    public string icon;
    public BuffType type;
    public bool isStack;
    public float tick;
    public float durationTime;
    public float lv_1;
    public float lv_2;
    public float lv_3;

    public int ID => id;
    public string Name => name;
    public string Description => description;
    public string Icon => icon;
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
                case "type":
                    buffInfo.type = Utils.StringToEnum<BuffType>(cell.value, BuffType.NULL);
                    break;
                case "isStack":
                    buffInfo.isStack = Utils.StringToBool(cell.value);
                    break;
                case "tick":
                    buffInfo.tick = Utils.StringToInt(cell.value);
                    break;
                case "durationTime":
                    buffInfo.durationTime = Utils.StringToFloat(cell.value);
                    break;
                case "lv_1":
                    buffInfo.lv_1 = Utils.StringToFloat(cell.value);
                    break;
                case "lv_2":
                    buffInfo.lv_2 = Utils.StringToFloat(cell.value);
                    break;
                case "lv_3":
                    buffInfo.lv_3 = Utils.StringToFloat(cell.value);
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
