using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HeroNameInfo : IInfo
{
    public int id;
    public string name;    // 닉네임 문자열
    public string description;
    public string icon;
    public int ID => id;
    public string Name => name;
    public string Description => description;
    public string Icon => icon;
}

[CreateAssetMenu(fileName = "HeroNameData", menuName = "Scriptable Object/New HeroNameData")]
public class HeroNameData : SheetDataReaderBase
{
    public List<HeroNameInfo> infoList = new List<HeroNameInfo>();

    private HeroNameInfo heronameInfo;

    public override void UpdateStat(List<GSTU_Cell> list)
    {
        heronameInfo = new HeroNameInfo();

        foreach (var cell in list)
        {
            switch (cell.columnId)
            {
                case "id":
                    heronameInfo.id = Utils.StringToInt(cell.value);
                    break;

                case "name":
                    heronameInfo.name = cell.value;
                    break;
            }
        }

        infoList.Add(heronameInfo);
    }

    public override void ClearInfoList()
    {
        infoList.Clear();
    }
}
