using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public class TrophyManager : MonoSingleton<TrophyManager>
{
    // 해당 Trophy의 id 클리어 여부
    public Dictionary<int, bool> trophyClear = new();
    // 해당 Trophy의 달성 여부
    public Dictionary<int, int> trophyCount = new(); // id, count
    // 클리어 여부를 확인하기 위해 unlockId -> TrophyId로 변환을 위함
    public Dictionary<int, int> unlockIdToTrophyIds = new();
    public Dictionary<int, IInfo> allCollections = new();

    void Start()
    {
        // TODO : 클리어 여부 및 Count 불러오기 SAVE LOAD
        SteamUserStats.SetAchievement("HELLOWORLD_1");
        SteamUserStats.StoreStats();

        foreach (var trophydic in DataManager.Instance.trophyDic)
        {
            // Trophy에서 저장되어 있는 ID가 아니면 추가해서 Clear상태 false로 만듬
            if (!trophyClear.ContainsKey(trophydic.Value.ID))
            {
                trophyClear[trophydic.Value.ID] = false;
            }

            // Trophy에서 저장되어 있는 ID가 아니면 추가해서 더해야할 Count를 0으로 만듬
            if (!trophyCount.ContainsKey(trophydic.Value.ID))
            {
                trophyCount[trophydic.Value.ID] = 0;
            }

            // 풀어야 할 ID와 업적 ID 매칭
            if (trophydic.Value.unLockID != 0)
            {
                unlockIdToTrophyIds[trophydic.Value.unLockID] = trophydic.Value.ID;
            }
        }

        CreateCollection(DataManager.Instance.monsterDic);
        CreateCollection(DataManager.Instance.queenAbilityDic);
        CreateCollection(DataManager.Instance.queenEnhanceDic);
        CreateCollection(DataManager.Instance.queenActiveSkillDic);
        CreateCollection(DataManager.Instance.heroStatusDic);
        CreateCollection(DataManager.Instance.heroAbilityDic);
    }

    /// <summary>
    /// 해금되어있다면 true 반환
    /// </summary>
    /// <param name="unlockID"></param>
    /// <returns></returns>
    public bool IsCollectionWithUnlockID(int unlockID)
    {
        // 만약 풀어야할 해금이 없다면 이미 해금된 상태임
        if (!unlockIdToTrophyIds.ContainsKey(unlockID))
        {
            return true;
        }

        // 해금상태를 확인
        return trophyClear[unlockIdToTrophyIds[unlockID]];
    }

    private void CreateCollection<T>(Dictionary<int, T> dataDic) where T : IInfo
    {
        foreach (var pair in dataDic)
        {
            allCollections[pair.Value.ID] = pair.Value;
        }
    }

    public bool IsRewardTrophy(int trophyId)
    {
        if (trophyClear[trophyId])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool GetRewardTrophy(int trophyId, Vector2 buttonPos)
    {
        var trophyInfo = DataManager.Instance.trophyDic[trophyId];
        if (trophyClear[trophyId])
        {
            Utils.Log("이미 클리어 된 업적입니다.");
            return true;
        }


        if (trophyInfo.maxCount > trophyCount[trophyId])
        {
            Utils.Log("클리어 되지 않은 상태라 리워드를 얻을 수 없습니다.");
            return false;
        }

        trophyClear[trophyId] = true;
        if (trophyInfo.unLockID != 0) // 돈이 아닌 해금
        {
            // 나중에 해금되는 것 만들 때 사용될 것(열쇠 아이콘)

        }
        else
        {
            int amount = 100;
            var goldCount = trophyInfo.reward / amount;
            // 골드 획득
            for (int i = 0; i < goldCount; i++)
            {
                Vector2 moneyPos = new Vector2(-865, 488);
                StaticUIManager.Instance.uiParticleLayer.ShowParticle("gameicon_tilemap-Sheet_1179", buttonPos, moneyPos, () =>
                {
                    GameManager.Instance.AddGold(amount);
                    StaticUIManager.Instance.hudLayer.GetHUD<MenuHUD>().GoldTextScaleUpAndDown();
                });
            }
            Invoke(nameof(DelayedStart), 2.3f);
            // UGSManager.Instance.SaveLoad.SaveAsync().Forget();
        }

        return true;
    }

    void DelayedStart()
    {
        UGSManager.Instance.SaveLoad.SaveAsync().Forget();
    }

    public void KillHeroId(int heroId)
    {
        RecordTrophyId(1000001);
        RecordTrophyId(1000004);
    }

    public void Levelup(int level)
    {
        RecordTrophyId(1000002, level);
        RecordTrophyId(1000003, level);
    }

    public void SummonMonsterId(int monsterId)
    {
        if (monsterId == (int)IDMonster.SKELETON_ARCHER)
            RecordTrophyId(1000005);

        if (monsterId == (int)IDMonster.ORC_NORMAL
        || monsterId == (int)IDMonster.ORC_WARRIOR
        || monsterId == (int)IDMonster.ORC_SHAMAN
        || monsterId == (int)IDMonster.ORC_WARRIOR2
        || monsterId == (int)IDMonster.ORC_BERSERKER
        || monsterId == (int)IDMonster.ORC_SHAMAN2)
            RecordTrophyId(1000006);
    }

    public void ClearGameEventId(int eventId)
    {
        RecordTrophyId(1000007);
        RecordTrophyId(1000008);
    }

    public void StartQueenId(int queenId)
    {
        if (queenId == (int)IDQueenStatus.ORC)
            RecordTrophyId(1000009);
    }

    public void UseSkillId(int skillId)
    {
        if (skillId == (int)IDQueenActiveSkill.SACRIFICE)
            RecordTrophyId(1000010);

        if (skillId == (int)IDQueenActiveSkill.SKELETON_LEGION)
            RecordTrophyId(1000011);
    }

    /// <summary>
    /// 업적 기록
    /// </summary>
    /// <param name="trophyId"></param>
    public void RecordTrophyId(int trophyId)
    {
        trophyCount[trophyId]++;
    }

    /// <summary>
    /// 업적 기록
    /// </summary>
    /// <param name="trophyId"></param>
    /// <param name="trophyId"></param>
    public void RecordTrophyId(int trophyId, int count)
    {
        trophyCount[trophyId] = count;
    }

    /// <summary>
    /// 업적 초기화
    /// </summary>
    public void ResetNonStackTrophy()
    {

        foreach (var trophydic in DataManager.Instance.trophyDic)
        {
            if (trophydic.Value.type == TrophyType.Stack) continue;

            // Stack이 아니라면 누적이 아니기 때문에 매판 초기화
            if (trophyCount.ContainsKey(trophydic.Value.ID))
            {
                // 도달하지 못했다면 0으로 초기화
                if (trophydic.Value.maxCount > trophyCount[trophydic.Value.ID])
                {
                    trophyCount[trophydic.Value.ID] = 0;
                }
            }
        }
    }
}
