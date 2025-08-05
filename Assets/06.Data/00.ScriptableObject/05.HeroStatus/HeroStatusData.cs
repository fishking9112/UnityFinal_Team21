using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;


public enum HeroType
{
    NULL,
    NORMAL,
    BOSS,
}

[Serializable]
public class HeroStatusInfo : BaseStatData, IInfo
{
    public int startLevel;
    public float detectedRange;
    public int custom;
    public HeroType heroType;
    public int weaponCount;
    public HeroStatusInfo() { }
    public HeroStatusInfo(HeroStatusInfo other) : base(other)
    {
        startLevel = other.startLevel;
        detectedRange = other.detectedRange;
        custom = other.custom;
        heroType = other.heroType;
        weaponCount = other.weaponCount;
    }
    public void Copy(HeroStatusInfo other)
    {
        id = other.id;
        name = other.name;
        description = other.description;
        health = other.health;
        moveSpeed = other.moveSpeed;
        reward = other.reward;

        startLevel = other.startLevel;
        detectedRange = other.detectedRange;
        custom = other.custom;
        weaponCount = other.weaponCount;
    }

    public int ID => id;
    public string Name => name;
    public string Description => description;
    public string Icon => string.Empty;
}


[CreateAssetMenu(fileName = "HeroStatusData", menuName = "Scriptable Object/New HeroStatusData")]
public class HeroStatusData : SheetDataReaderBase
{
    public List<HeroStatusInfo> infoList = new List<HeroStatusInfo>();

    private HeroStatusInfo heroStatusInfo;
    public override void UpdateStat(List<GSTU_Cell> list)
    {
        heroStatusInfo = new HeroStatusInfo();

        foreach (var cell in list)
        {
            switch (cell.columnId)
            {
                case "id":
                    heroStatusInfo.id = Utils.StringToInt(cell.value);
                    break;
                case "name":
                    heroStatusInfo.name = cell.value;
                    break;
                case "description":
                    heroStatusInfo.description = cell.value;
                    break;
                case "startLevel":
                    heroStatusInfo.startLevel = Utils.StringToInt(cell.value);
                    break;
                
                case "hp":
                    heroStatusInfo.health = Utils.StringToFloat(cell.value);
                    break;
                case "detectedRange":
                    heroStatusInfo.detectedRange = Utils.StringToFloat(cell.value);
                    break;
                case "moveSpeed":
                    heroStatusInfo.moveSpeed = Utils.StringToFloat(cell.value);
                    break;
                case "reward":
                    heroStatusInfo.reward = Utils.StringToFloat(cell.value);
                    break;
                case "custom":
                    heroStatusInfo.custom = Utils.StringToInt(cell.value);
                    break;
                case "heroType":
                    heroStatusInfo.heroType = Utils.StringToEnum<HeroType>(cell.value, HeroType.NULL);
                    break;
                case "weaponCount":
                    heroStatusInfo.weaponCount = Utils.StringToInt(cell.value);
                    break;
            }
        }
        infoList.Add(heroStatusInfo);
    }

    public override void ClearInfoList()
    {
        infoList.Clear();
    }
}
