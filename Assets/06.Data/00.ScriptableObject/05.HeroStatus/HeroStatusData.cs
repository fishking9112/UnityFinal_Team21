using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HeroStatusInfo: BaseStatData, IInfo
{
    public int id;
    public string name;
    public string description;
    public int startLevel;
    public int[] weapon;
    public int[] weaponLevel;
    public float detectedRange;
    public float reward;
    public int custom;
    public HeroStatusInfo() { }
    public HeroStatusInfo(HeroStatusInfo other) : base(other)
    {
        id = other.id;
        name = other.name;
        description = other.description;
        startLevel = other.startLevel;
        weapon = other.weapon;
        weaponLevel = other.weaponLevel;
        health = other.health;
        detectedRange = other.detectedRange;
        moveSpeed = other.moveSpeed;
        reward = other.reward;
        custom = other.custom;
    }
    public void Copy(HeroStatusInfo other)
    {
        id = other.id;
        name = other.name;
        description = other.description;
        startLevel = other.startLevel;
        weapon = other.weapon;
        weaponLevel = other.weaponLevel;
        health = other.health;
        detectedRange = other.detectedRange;
        moveSpeed = other.moveSpeed;
        reward = other.reward;
        custom = other.custom;
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
                case "weapon":
                    heroStatusInfo.weapon = Utils.StringToIntArr(cell.value);
                    break;
                case "weaponLevel":
                    heroStatusInfo.weaponLevel = Utils.StringToIntArr(cell.value);
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
            }
        }
        infoList.Add(heroStatusInfo);
    }

    public override void ClearInfoList()
    {
        infoList.Clear();
    }
}
