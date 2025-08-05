using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HeroAbilityLevelUpInfo : IInfo
{
    public int id;
    public string name;
    public float damage_LevelUp;
    public float delay_LevelUp;
    public float piercing_LevelUp;
    public Vector3 size_LevelUp;
    public float speed_LevelUp;
    public float rotateSpeed_LevelUp;
    public float duration_LevelUp;
    public float count_LevelUp;
    public float countDelay_LevelUp;

    public int ID => id;
    public string Name => name;
    public string Description => string.Empty;
    public string Icon => string.Empty;
}

[CreateAssetMenu(fileName = "HeroAbilityLevelUpData", menuName = "Scriptable Object/New HeroAbilityLevelUpData")]
public class HeroAbilityLevelUpData : SheetDataReaderBase
{
    public List<HeroAbilityLevelUpInfo> infoList = new List<HeroAbilityLevelUpInfo>();

    private HeroAbilityLevelUpInfo heroAbilityLevelUpInfo;

    public override void UpdateStat(List<GSTU_Cell> list)
    {
        heroAbilityLevelUpInfo = new HeroAbilityLevelUpInfo();

        foreach (var cell in list)
        {
            switch (cell.columnId)
            {
                case "id":
                    heroAbilityLevelUpInfo.id = Utils.StringToInt(cell.value);
                    break;
                case "name":
                    heroAbilityLevelUpInfo.name = cell.value;
                    break;
                case "damage_LevelUp":
                    heroAbilityLevelUpInfo.damage_LevelUp = Utils.StringToFloat(cell.value);
                    break;
                case "delay_LevelUp":
                    heroAbilityLevelUpInfo.delay_LevelUp = Utils.StringToFloat(cell.value);
                    break;
                case "piercing_LevelUp":
                    heroAbilityLevelUpInfo.piercing_LevelUp = Utils.StringToFloat(cell.value);
                    break;
                case "size_LevelUp":
                    heroAbilityLevelUpInfo.size_LevelUp = Utils.StringToVector3(cell.value);
                    break;
                case "speed_LevelUp":
                    heroAbilityLevelUpInfo.speed_LevelUp = Utils.StringToFloat(cell.value);
                    break;
                case "rotateSpeed_LevelUp":
                    heroAbilityLevelUpInfo.rotateSpeed_LevelUp = Utils.StringToFloat(cell.value);
                    break;
                case "duration_LevelUp":
                    heroAbilityLevelUpInfo.duration_LevelUp = Utils.StringToFloat(cell.value);
                    break;
                case "count_LevelUp":
                    heroAbilityLevelUpInfo.count_LevelUp = Utils.StringToFloat(cell.value);
                    break;
                case "countDelay_LevelUp":
                    heroAbilityLevelUpInfo.countDelay_LevelUp = Utils.StringToFloat(cell.value);
                    break;
            }
        }
        infoList.Add(heroAbilityLevelUpInfo);
    }

    public override void ClearInfoList()
    {
        infoList.Clear();
    }
}
