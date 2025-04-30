using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrophyManager : MonoSingleton<TrophyManager>
{
    // 해당 Trophy의 id 클리어 여부
    public Dictionary<int, bool> trophyClear = new();
    // 해당 Trophy의 달성 여부
    public Dictionary<int, int> trophyCount = new();
    // 클리어 여부를 확인하기 위해 unlockId -> TrophyId로 변환을 위함
    public Dictionary<int, int> unlockIdToTrophyIds = new();
    public Dictionary<int, IInfo> allCollections = new();

    void Start()
    {
        // TODO : 클리어 여부 및 Count 불러오기 SAVE LOAD


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
}
