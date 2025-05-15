using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum QueenEnhanceType
{
    NULL,
    Point,
    QueenPassive,
    MonsterPassive,
    AddSkill
}
public enum ValueType
{
    NULL,
    Queen,
    Hp,
    Attack,
    MoveSpeed,
    Default
}

[Serializable]
public class QueenEnhanceInfo : IInfo
{
    public int id;
    public string name;
    public string description;
    public string icon;
    public int maxLevel;
    public float state_Base;
    public int state_LevelUp;
    public QueenEnhanceType type;
    public MonsterBrood brood;
    public ValueType valueType;
    public int skill_ID;

    public int ID => id;
    public string Name => name;
    public string Description => description;
    public string Icon => icon;
    public int Skill_ID => skill_ID;
}

[CreateAssetMenu(fileName = "QueenEnhanceData", menuName = "Scriptable Object/New QueenEnhanceData")]
public class QueenEnhanceData : SheetDataReaderBase
{
    public List<QueenEnhanceInfo> infoList = new List<QueenEnhanceInfo>();

    private QueenEnhanceInfo queenEnhanceInfo;

    public override void UpdateStat(List<GSTU_Cell> list)
    {
        queenEnhanceInfo = new QueenEnhanceInfo();

        foreach (var cell in list)
        {
            switch (cell.columnId)
            {
                case "id":
                    queenEnhanceInfo.id = Utils.StringToInt(cell.value);
                    break;
                case "name":
                    queenEnhanceInfo.name = cell.value;
                    break;
                case "description":
                    queenEnhanceInfo.description = cell.value;
                    break;
                case "icon":
                    queenEnhanceInfo.icon = cell.value;
                    break;
                case "maxLevel":
                    queenEnhanceInfo.maxLevel = Utils.StringToInt(cell.value);
                    break;
                case "state_Base":
                    queenEnhanceInfo.state_Base = Utils.StringToFloat(cell.value);
                    break;
                case "state_LevelUp":
                    queenEnhanceInfo.state_LevelUp = Utils.StringToInt(cell.value);
                    break;
                case "type":
                    queenEnhanceInfo.type = Utils.StringToEnum<QueenEnhanceType>(cell.value, QueenEnhanceType.NULL);
                    break;
                case "brood":
                    queenEnhanceInfo.brood = Utils.StringToEnum<MonsterBrood>(cell.value, MonsterBrood.NULL);
                    break;
                case "valueType":
                    queenEnhanceInfo.valueType = Utils.StringToEnum<ValueType>(cell.value, ValueType.NULL);
                    break;
                case "skill_ID":
                    queenEnhanceInfo.skill_ID = Utils.StringToInt(cell.value);
                    break;
            }
        }
        infoList.Add(queenEnhanceInfo);
    }

    public override void ClearInfoList()
    {
        infoList.Clear();
    }
}
