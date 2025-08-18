using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class TrophyManager : MonoSingleton<TrophyManager>
{
    // 해당 Trophy의 id 클리어 여부
    public Dictionary<int, bool> trophyClear = new();
    // 해당 Trophy의 달성 여부
    public Dictionary<int, int> trophyCount = new(); // id, count
    // 클리어 여부를 확인하기 위해 unlockId -> TrophyId로 변환을 위함
    public Dictionary<int, int> unlockIdToTrophyIds = new();
    public Dictionary<int, IInfo> allCollections = new();

    private void Start()
    {
        startAsync().Forget();
    }

    private async UniTaskVoid startAsync()
    {
        await UniTask.WaitUntil(() => StoveGameServiceManager.Instance != null && StoveGameServiceManager.Instance.IsLoaded);
        InitializeTrophyData();
    }

    private void InitializeTrophyData()
    {
        foreach (var trophydic in DataManager.Instance.trophyDic)
        {
            if (!trophyClear.ContainsKey(trophydic.Value.ID))
                trophyClear[trophydic.Value.ID] = false;

            if (!trophyCount.ContainsKey(trophydic.Value.ID))
                trophyCount[trophydic.Value.ID] = 0;

            if (trophydic.Value.unLockID != 0)
                unlockIdToTrophyIds[trophydic.Value.unLockID] = trophydic.Value.ID;
        }

        CreateCollection(DataManager.Instance.monsterDic);
        CreateCollection(DataManager.Instance.queenAbilityDic);
        CreateCollection(DataManager.Instance.queenEnhanceDic);
        CreateCollection(DataManager.Instance.queenActiveSkillDic);
        CreateCollection(DataManager.Instance.heroStatusDic);
        CreateCollection(DataManager.Instance.heroAbilityDic);

        Utils.Log("TrophyManager 초기화 완료");
    }

    public void UpdateTrophyRedDotUI()
    {
        if (StaticUIManager.Instance == null || StaticUIManager.Instance.hudLayer == null)
            return;

        var menuHUD = StaticUIManager.Instance.hudLayer.GetHUD<MenuHUD>();
        if (menuHUD != null)
        {
            bool showRedDot = HasRewardableTrophy();
            menuHUD.redDot_Notification.SetActive(showRedDot);
        }
    }

    public bool HasRewardableTrophy()
    {
        foreach (var kvp in trophyCount)
        {
            int trophyId = kvp.Key;
            int count = kvp.Value;

            if (DataManager.Instance.trophyDic.TryGetValue(trophyId, out var trophyInfo))
            {
                if (!trophyClear[trophyId] && count >= trophyInfo.maxCount)
                {
                    return true; // 아직 수령하지 않았고 조건도 만족한 업적이 있음
                }
            }
        }

        return false;
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
                    int money = amount;
                    // 골드 획득
                    GameManager.Instance.AddGold(money);
                    if (StaticUIManager.Instance != null && StaticUIManager.Instance.hudLayer != null)
                    {
                        var menuHUD = StaticUIManager.Instance.hudLayer.GetHUD<MenuHUD>();
                        if (menuHUD != null)
                        {
                            menuHUD.GoldTextScaleUpAndDown();
                        }
                    }
                });
            }
            Invoke(nameof(DelayedStart), 2.3f);
            StoveGameServiceManager.Instance.SaveLoad.SaveAsync().Forget();
        }

        UpdateTrophyRedDotUI();
        return true;
    }

    void DelayedStart()
    {
        StoveGameServiceManager.Instance.SaveLoad.SaveAsync().Forget();
    }

    public void KillHeroId(int heroId)
    {
        RecordTrophyId(1000001);
        RecordTrophyId(1000004);
    }

    public void Levelup(int level)
    {
        UpdateProgressIfNotCleared(1000002, level);
        UpdateProgressIfNotCleared(1000003, level);
    }

    private void UpdateProgressIfNotCleared(int trophyId, int value)
    {
        if (!trophyCount.ContainsKey(trophyId)) return;

        var trophyInfo = DataManager.Instance.trophyDic[trophyId];

        // 이미 달성 조건을 만족했다면 갱신하지 않음
        if (trophyCount[trophyId] >= trophyInfo.maxCount)
            return;

        // 아직 달성 조건을 만족하지 않았다면 갱신
        RecordTrophyId(trophyId, value);
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

            int trophyId = trophydic.Value.ID;

            if (trophyCount.ContainsKey(trophyId))
            {
                // 1. 이미 달성 조건을 만족했으면(보상 수령 여부와 무관) 초기화하지 않음
                if (trophyCount[trophyId] >= trophydic.Value.maxCount)
                    continue;

                // 2. 달성하지 못했으면 초기화
                trophyCount[trophyId] = 0;
            }
        }
    }
}
