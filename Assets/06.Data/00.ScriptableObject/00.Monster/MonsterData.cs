using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterAttackType
{
    NULL,
    MELEE,
    RANGED,
    MAGIC,
    SHURIKEN
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
    public float cost;
    public string outfit;
    public string icon;
    public float attack;
    public float attackRange;
    public float attackSpeed;

    public MonsterAttackType monsterAttackType;
    public MonsterBrood monsterBrood;
    public string projectile;
    public int tire;
    public int preNode;
    public MonsterInfo() { }
    public MonsterInfo(MonsterInfo other) : base(other)
    {
        cost = other.cost;
        outfit = other.outfit;
        icon = other.icon;
        attack = other.attack;
        attackRange = other.attackRange;
        attackSpeed = other.attackSpeed;
        monsterAttackType = other.monsterAttackType;
        monsterBrood = other.monsterBrood;
        projectile = other.projectile;
        tire = other.tire;
        preNode = other.preNode;
    }
    public void Copy(MonsterInfo other)
    {

        id = other.id;
        name = other.name;
        description = other.description;
        health = other.health;
        moveSpeed = other.moveSpeed;
        reward = other.reward;

        cost = other.cost;
        outfit = other.outfit;
        icon = other.icon;
        attack = other.attack;
        attackRange = other.attackRange;
        attackSpeed = other.attackSpeed;
        monsterAttackType = other.monsterAttackType;
        monsterBrood = other.monsterBrood;
        projectile = other.projectile;
        tire = other.tire;
        preNode = other.preNode;
    }
    public int ID => id;
    public string Name => name;
    public string Description => description;
    public string Icon => icon;
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
                case "icon":
                    monsterInfo.icon = cell.value;
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