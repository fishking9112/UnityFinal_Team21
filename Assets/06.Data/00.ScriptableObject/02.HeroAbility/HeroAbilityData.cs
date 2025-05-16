using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum HeroAbilityType
{
    NULL,
    MELEE,
    RANGED,
    REVOLUTION,
    AREA,
    AXE,
    CHAIN,
    TARGETTING,
    HOLD,
}

[Serializable]
public class HeroAbilityInfo : IInfo
{
    public int id;
    public string name;
    public string description;
    public string icon;
    public int maxLevel;
    public Vector3 pivot;
    public float damage_Base;
    public float delay_Base;
    public int piercing_Base;
    public float knockback;
    public Vector3 size_Base;
    public HeroAbilityType type;
    public float speed_Base;
    public float rotateSpeed_Base;
    public float duration_Base;
    public int count_Base;
    public float countDelay_Base;
    public float damage_Delay;
    public float damage_Range;

    public int ID => id;
    public string Name => name;
    public string Description => description;
    public string Icon => icon;
}

[CreateAssetMenu(fileName = "HeroAbilityData", menuName = "Scriptable Object/New HeroAbilityData")]
public class HeroAbilityData : SheetDataReaderBase
{
    public List<HeroAbilityInfo> infoList = new List<HeroAbilityInfo>();

    private HeroAbilityInfo heroAbilityInfo;

    public override void UpdateStat(List<GSTU_Cell> list)
    {
        heroAbilityInfo = new HeroAbilityInfo();

        foreach (var cell in list)
        {
            switch (cell.columnId)
            {
                case "id":
                    heroAbilityInfo.id = Utils.StringToInt(cell.value);
                    break;
                case "name":
                    heroAbilityInfo.name = cell.value;
                    break;
                case "description":
                    heroAbilityInfo.description = cell.value;
                    break;
                case "icon":
                    heroAbilityInfo.icon = cell.value;
                    break;
                case "maxLevel":
                    heroAbilityInfo.maxLevel = Utils.StringToInt(cell.value);
                    break;
                case "pivot":
                    heroAbilityInfo.pivot = Utils.StringToVector3(cell.value);
                    break;
                case "damage_Base":
                    heroAbilityInfo.damage_Base = Utils.StringToFloat(cell.value);
                    break;
                case "delay_Base":
                    heroAbilityInfo.delay_Base = Utils.StringToFloat(cell.value);
                    break;
                case "piercing_Base":
                    heroAbilityInfo.piercing_Base = Utils.StringToInt(cell.value);
                    break;
                case "knockback":
                    heroAbilityInfo.knockback = Utils.StringToFloat(cell.value);
                    break;
                case "size_Base":
                    heroAbilityInfo.size_Base = Utils.StringToVector3(cell.value);
                    break;
                case "type":
                    heroAbilityInfo.type = Utils.StringToEnum<HeroAbilityType>(cell.value, HeroAbilityType.NULL);
                    break;
                case "speed_Base":
                    heroAbilityInfo.speed_Base = Utils.StringToFloat(cell.value);
                    break;
                case "rotateSpeed_Base":
                    heroAbilityInfo.rotateSpeed_Base = Utils.StringToFloat(cell.value);
                    break;
                case "duration_Base":
                    heroAbilityInfo.duration_Base = Utils.StringToFloat(cell.value);
                    break;
                case "count_Base":
                    heroAbilityInfo.count_Base = Utils.StringToInt(cell.value);
                    break;
                case "countDelay_Base":
                    heroAbilityInfo.countDelay_Base = Utils.StringToFloat(cell.value);
                    break;
                case "damage_Delay":
                    heroAbilityInfo.damage_Delay = Utils.StringToFloat(cell.value);
                    break;
                case "damage_Range":
                    heroAbilityInfo.damage_Range = Utils.StringToFloat(cell.value);
                    break;
            }
        }
        infoList.Add(heroAbilityInfo);
    }

    public override void ClearInfoList()
    {
        infoList.Clear();
    }
}
