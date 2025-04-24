using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterAttackType
{
    NULL,
    MELEE,
    RANGED,
}

public enum MonsterBrood
{
    NULL,
    None,
    Slime,
    Orc,
    Skeleton,
}

[Serializable]
public class MonsterInfo : BaseStatData, IInfo
{
    public int id;
    public string name;
    public string description;
    public float cost;
    public float reward;
    public string outfit;

    public MonsterAttackType monsterAttackType;
    public MonsterBrood monsterBrood;
    public string projectile;
    public int tire;
    public int preNode;
    public MonsterInfo() { }
    public MonsterInfo(MonsterInfo other) : base(other)
    {
        id = other.id;
        name = other.name;
        description = other.description;
        cost = other.cost;
        reward = other.reward;
        outfit = other.outfit;
        monsterAttackType = other.monsterAttackType;
        monsterBrood = other.monsterBrood;
        projectile = other.projectile;
    }
    public void Copy(MonsterInfo other)
    {
        id = other.id;
        name = other.name;
        description = other.description;
        cost = other.cost;
        reward = other.reward;
        outfit = other.outfit;
        monsterAttackType = other.monsterAttackType;
        monsterBrood = other.monsterBrood;
        projectile = other.projectile;
    }
    public int ID => id;
}

[CreateAssetMenu(fileName = "MonsterData", menuName = "Scriptable Object/New MonsterData")]
public class MonsterData : SheetDataReaderBase
{
    public List<MonsterInfo> infoList = new List<MonsterInfo>();

    private MonsterInfo monsterInfo;

    /// <summary>
    /// 구글 시트에 저장된 데이터를 읽어옴
    /// </summary>
    /// <param name="list"> 구글 시트 cell 리스트 </param>
    public override void UpdateStat(List<GSTU_Cell> list)
    {
        monsterInfo = new MonsterInfo();

        foreach (var cell in list)
        {
            switch (cell.columnId)
            {
                case "id":
                    monsterInfo.id = Utils.StringToInt(cell.value);
                    break;
                case "name":
                    monsterInfo.name = cell.value;
                    break;
                case "description":
                    monsterInfo.description = cell.value;
                    break;
                case "health":
                    monsterInfo.health = Utils.StringToFloat(cell.value);
                    break;
                case "defence":
                    monsterInfo.defence = Utils.StringToFloat(cell.value);
                    break;
                case "cost":
                    monsterInfo.cost = Utils.StringToFloat(cell.value);
                    break;
                case "moveSpeed":
                    monsterInfo.moveSpeed = Utils.StringToFloat(cell.value);
                    break;
                case "attack":
                    monsterInfo.attack = Utils.StringToFloat(cell.value);
                    break;
                case "attackRange":
                    monsterInfo.attackRange = Utils.StringToFloat(cell.value);
                    break;
                case "attackSpeed":
                    monsterInfo.attackSpeed = Utils.StringToFloat(cell.value);
                    break;
                case "reward":
                    monsterInfo.reward = Utils.StringToFloat(cell.value);
                    break;
                case "brood":
                    monsterInfo.monsterBrood = Utils.StringToEnum<MonsterBrood>(cell.value, MonsterBrood.NULL);
                    break;
                case "outfit":
                    monsterInfo.outfit = cell.value;
                    break;
                case "type":
                    monsterInfo.monsterAttackType = Utils.StringToEnum<MonsterAttackType>(cell.value, MonsterAttackType.NULL);
                    break;
                case "projectile":
                    monsterInfo.projectile = cell.value;
                    break;
                case "tire":
                    monsterInfo.tire = Utils.StringToInt(cell.value);
                    break;
                case "preNode":
                    monsterInfo.preNode = Utils.StringToInt(cell.value);
                    break;
            }
        }
        infoList.Add(monsterInfo);
    }

    public override void ClearInfoList()
    {
        infoList.Clear();
    }
}