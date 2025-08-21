using System.Collections.Generic;
using UnityEngine;

// 각 몬스터의 스택 정보를 담는 클래스입니다.
public class MonsterStackInfo
{
    public int ID;
    public int CurrentStacks;
    public int MaxStacks;
    public float CurrentCooldown;
    public float StackRegenSpeed;
}

public class MonsterSummonManager : MonoSingleton<MonsterSummonManager>
{
    // 몬스터 ID를 키로 하여 스택 정보를 관리하는 딕셔너리
    public Dictionary<int, MonsterStackInfo> monsterStacks = new Dictionary<int, MonsterStackInfo>();
    private float maxCooldown = 10f;

    public bool isInitialized = false;

    /// <summary>
    /// 몬스터 스택 정보를 초기화
    /// </summary>
    public void Initialize()
    {
        if (isInitialized) return;

        // DataManager에서 몬스터 데이터를 가져와 스택 정보 설정
        var monsterStatDic = DataManager.Instance.queenAbilityMonsterStatDic;
        foreach (var monsterData in monsterStatDic.Values)
        {
            monsterStacks[monsterData.ID] = new MonsterStackInfo
            {
                ID = monsterData.ID,
                MaxStacks = monsterData.maxMonsterStack, // 예시: 최대 3스택
                CurrentStacks = monsterData.maxMonsterStack,
                StackRegenSpeed = monsterData.stackRegenSpeed, // 예시: 10을 기준으로 회복됨
                CurrentCooldown = 0
            };
        }

        isInitialized = true;
        Utils.Log("MonsterSummonManager가 초기화되었습니다.");
    }

    void Update()
    {
        if (!isInitialized) return;

        // 매 프레임 쿨타임을 계산하여 스택을 충전
        foreach (var stackInfo in monsterStacks.Values)
        {
            if (stackInfo.CurrentStacks < stackInfo.MaxStacks)
            {
                stackInfo.CurrentCooldown += Time.deltaTime * stackInfo.StackRegenSpeed;
                if (maxCooldown <= stackInfo.CurrentCooldown)
                {
                    stackInfo.CurrentStacks++;
                    stackInfo.CurrentCooldown = 0;
                }
            }
        }
    }

    /// <summary>
    /// 몬스터를 소환할 수 있는지 확인
    /// </summary>
    /// <param name="monsterId">몬스터 ID</param>
    /// <returns>소환 가능하면 true</returns>
    public bool CanSummon(int monsterId)
    {
        return monsterStacks.ContainsKey(monsterId) && monsterStacks[monsterId].CurrentStacks > 0;
    }

    /// <summary>
    /// 몬스터 소환 시 스택 관리
    /// </summary>
    /// <param name="monsterId">몬스터 ID</param>
    public void ConsumeStack(int monsterId)
    {
        if (CanSummon(monsterId))
        {
            monsterStacks[monsterId].CurrentStacks--;
            // 스택이 0이 되면 쿨타임 계산 시작
            if (monsterStacks[monsterId].CurrentStacks < monsterStacks[monsterId].MaxStacks)
            {
                if (monsterStacks[monsterId].CurrentCooldown <= 0)
                {
                    monsterStacks[monsterId].CurrentCooldown = 0;
                }
            }
        }
    }

    /// <summary>
    /// UI 표시 등을 위해 현재 몬스터의 스택 정보 전달
    /// </summary>
    /// <param name="monsterId">몬스터 ID</param>
    public MonsterStackInfo GetStackInfo(int monsterId)
    {
        if (monsterStacks.ContainsKey(monsterId))
        {
            return monsterStacks[monsterId];
        }
        return null;
    }


    /// <summary>
    /// UI 표시 등을 위해 현재 몬스터의 스택 정보 전달
    /// </summary>
    /// <param name="monsterId">몬스터 ID</param>
    public float GetStackPercent(int monsterId)
    {
        if (monsterStacks.ContainsKey(monsterId))
        {
            return monsterStacks[monsterId].CurrentCooldown / maxCooldown;
        }
        return 0f;
    }
}