using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum HeroAbilityType
{
    MELEE,
    RANGED,
    REVOLUTION,
    AREA,
    AXE,
}

[Serializable]
public class HeroAbilityInfo : IInfo
{
    public int id;
    public string name;
    public string description;
    public string icon;
    public int maxLevel;
    public float damage;
    public float delay;
    public float range;
    public Vector3 pivot;
    public Vector3 size;
    public float duration;
    public int piercing;

    public HeroAbilityType type;
    public float speed;
    public float rotateSpeed;

    public int ID => id;
}

[CreateAssetMenu(fileName ="HeroAbilityData",menuName ="Scriptable Object/New HeroAbilityData")]
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
                    heroAbilityInfo.id = int.Parse(cell.value);
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
                    heroAbilityInfo.maxLevel = int.Parse(cell.value);
                    break;
                case "damage":
                    heroAbilityInfo.damage = float.Parse(cell.value);
                    break;
                case "delay":
                    heroAbilityInfo.delay = float.Parse(cell.value);
                    break;
                case "range":
                    heroAbilityInfo.range = float.Parse(cell.value);
                    break;
                case "pivot":
                    heroAbilityInfo.pivot = Utils.StringToVector3(cell.value);
                    break;
                case "size":
                    heroAbilityInfo.size = Utils.StringToVector3(cell.value);
                    break;
                case "piercing":
                    heroAbilityInfo.piercing = int.Parse(cell.value);
                    break;
                case "type":
                    heroAbilityInfo.type = (HeroAbilityType)Enum.Parse(typeof(HeroAbilityType), cell.value);
                    break;
                case "speed":
                    if (heroAbilityInfo.type == HeroAbilityType.RANGED || heroAbilityInfo.type == HeroAbilityType.AXE)
                    {
                        heroAbilityInfo.speed = float.Parse(cell.value);
                    }
                    break;
                case "rotateSpeed":
                    if (heroAbilityInfo.type == HeroAbilityType.AXE)
                    {
                        heroAbilityInfo.rotateSpeed = float.Parse(cell.value);
                    }
                    break;
            }
        }
        infoList.Add(heroAbilityInfo);
    }
}
