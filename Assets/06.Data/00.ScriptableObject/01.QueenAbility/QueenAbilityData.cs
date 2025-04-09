using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelInfo
{
    public int cost;
    public int eff;
}

[Serializable]
public class QueenAbilityInfo
{
    public int id;
    public string name;
    public string description;
    public int maxLevel;
    public LevelInfo[] levelInfo;
}

[CreateAssetMenu(fileName = "QueenAbilityData", menuName = "Scriptable Object/New QueenAbilityData")]
public class QueenAbilityData : SheetDataReaderBase
{
    public List<QueenAbilityInfo> infoList = new List<QueenAbilityInfo>();

    private QueenAbilityInfo queenAbilityInfo;

    public override void UpdateStat(List<GSTU_Cell> list)
    {
        queenAbilityInfo = new QueenAbilityInfo();

        foreach(var cell in list)
        {
            
            switch (cell.columnId)
            {
                case "id":
                    queenAbilityInfo.id = int.Parse(cell.value);
                    break;
                case "name":
                    queenAbilityInfo.name = cell.value;
                    break;
                case "description":
                    queenAbilityInfo.description = cell.value;
                    break;
                case "maxLevel":
                    queenAbilityInfo.maxLevel = int.Parse(cell.value);
                    queenAbilityInfo.levelInfo = new LevelInfo[queenAbilityInfo.maxLevel];
                    for(int i = 0; i < queenAbilityInfo.maxLevel; i++)
                    {
                        queenAbilityInfo.levelInfo[i] = new LevelInfo();
                    }
                    break;
                case "cost_1":
                case "cost_2":
                case "cost_3":
                case "cost_4":
                case "cost_5":
                    int levelIndex = int.Parse(cell.columnId.Split('_')[1]) - 1;
                    if (levelIndex < queenAbilityInfo.maxLevel)
                    {
                        if (int.TryParse(cell.value, out int costValue))
                        {
                            queenAbilityInfo.levelInfo[levelIndex].cost = costValue;
                        }
                    }
                    break;
                case "eff_1":
                case "eff_2":
                case "eff_3":
                case "eff_4":
                case "eff_5":
                    levelIndex = int.Parse(cell.columnId.Split('_')[1]) - 1;
                    if (levelIndex < queenAbilityInfo.maxLevel)
                    {
                        if (int.TryParse(cell.value, out int effValue))
                        {
                            queenAbilityInfo.levelInfo[levelIndex].eff = effValue;
                        }
                    }
                    break;
            }
        }
        infoList.Add(queenAbilityInfo);
    }
}
