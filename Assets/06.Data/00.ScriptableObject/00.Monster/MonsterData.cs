using GoogleSheetsToUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterType
{
    MELEE,
    RANGED,
}

[Serializable]
public class MonsterInfo
{
    public int id;
    public string name;
    public string description;
    public float health;
    public float defence;
    public float cost;
    public float moveSpeed;
    public float attack;
    public float attackRange;
    public float attackSpeed;
    public float reward;
    public string outfit;

    public MonsterType type;
    public string projectile;
}

[CreateAssetMenu(fileName ="MonsterData", menuName ="Scriptable Object/New MonsterData")]
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

        foreach(var cell in list)
        {
            switch (cell.columnId)
            {
                case "id":
                    monsterInfo.id = int.Parse(cell.value);
                    break;
                case "name":
                    monsterInfo.name = cell.value;
                    break;
                case "description":
                    monsterInfo.description = cell.value;
                    break;
                case "health":
                    monsterInfo.health = float.Parse(cell.value);
                    break;
                case "defence":
                    monsterInfo.defence = float.Parse(cell.value);
                    break;
                case "cost":
                    monsterInfo.cost = float.Parse(cell.value);
                    break;
                case "moveSpeed":
                    monsterInfo.moveSpeed = float.Parse(cell.value);
                    break;
                case "attack":
                    monsterInfo.attack = float.Parse(cell.value);
                    break;
                case "attackRange":
                    monsterInfo.attackRange = float.Parse(cell.value);
                    break;
                case "attackSpeed":
                    monsterInfo.attackSpeed = float.Parse(cell.value);
                    break;
                case "reward":
                    monsterInfo.reward = float.Parse(cell.value);
                    break;
                case "outfit":
                    monsterInfo.outfit = cell.value;
                    break;
                case "type":
                    monsterInfo.type = (MonsterType)Enum.Parse(typeof(MonsterType), cell.value);
                    break;
                case "projectile":
                    if(monsterInfo.type == MonsterType.RANGED)
                    {
                        monsterInfo.projectile = cell.value;
                    }
                    break;
            }
        }
        infoList.Add(monsterInfo);
    }
}