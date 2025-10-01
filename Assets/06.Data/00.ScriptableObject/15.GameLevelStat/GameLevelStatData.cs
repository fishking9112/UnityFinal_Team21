using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameLevelStatInfo : IInfo
{
    public int id;
    public string gameLevel;
    public int heroHealthStat;
    public int monsterHealthStat;
    public string description;
    public string icon;

    public int ID => id;
    public string Name => gameLevel;
    public string Description => description;
    public string Icon => icon;
}

[CreateAssetMenu(fileName = "GameLevelStatData", menuName = "Scriptable Object/New GameLevelStatData")]
public class GameLevelStatData : SheetDataReaderBase
{
    public List<GameLevelStatInfo> infoList = new List<GameLevelStatInfo>();

    private GameLevelStatInfo gameLevelStatInfo;

    public override void UpdateStat(List<GSTU_Cell> list)
    {
        gameLevelStatInfo = new GameLevelStatInfo();

        foreach (var cell in list)
        {
            switch (cell.columnId)
            {
                case "id":
                    gameLevelStatInfo.id = Utils.StringToInt(cell.value);
                    break;
                case "GameLevel":
                    gameLevelStatInfo.gameLevel = cell.value;
                    break;
                case "HeroHealthStat":
                    gameLevelStatInfo.heroHealthStat = Utils.StringToInt(cell.value);
                    break;
                case "MonsterHealthStat":
                    gameLevelStatInfo.monsterHealthStat = Utils.StringToInt(cell.value);
                    break;
            }
        }

        infoList.Add(gameLevelStatInfo);
    }

    public override void ClearInfoList()
    {
        infoList.Clear();
    }
}
