using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HeroNameInfo : IInfo
{
    public string name;    // 닉네임 문자열

    public string Name => name;

    public int ID => throw new NotImplementedException();

    public string Description => throw new NotImplementedException();

    public string Icon => throw new NotImplementedException();
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
