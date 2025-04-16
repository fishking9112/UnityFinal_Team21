using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QueenEnhanceManager : MonoSingleton<QueenEnhanceManager>
{
    private Dictionary<int, int> acquiredEnhanceLevels = new Dictionary<int, int>();
    public IReadOnlyDictionary<int, int> AcquiredEnhanceLevels => acquiredEnhanceLevels;

    private QueenEnhanceUIController queenEnhanceUIController;
    public QueenEnhanceUIController QueenEnhanceUIController => queenEnhanceUIController;

    /// <summary>
    /// 외부에서 호출되는 강화 진입 함수
    /// </summary>
    public void ActivateEnhance()
    {
        Time.timeScale = 0;
        var randomOptions = GetRandomInhanceOptions();

        // UIController에게 전달
        QueenEnhanceUIController.ShowSelectUI(randomOptions);
    }

    /// <summary>
    /// 현재 전체 강화 목록 중에서 MaxLevel을 제외한 것들 중에서 3개 고른다.
    /// </summary>
    private List<QueenEnhanceInfo> GetRandomInhanceOptions()
    {
        List<QueenEnhanceInfo> availableList = new List<QueenEnhanceInfo>();

        foreach (var pair in DataManager.Instance.queenEnhanceDic)
        {
            int id = pair.Key;
            QueenEnhanceInfo info = pair.Value;

            int currentLevel = 0;
            acquiredEnhanceLevels.TryGetValue(id, out currentLevel);

            if (currentLevel < info.maxLevel)
            {
                availableList.Add(info);
            }
        }

        List<QueenEnhanceInfo> result = new List<QueenEnhanceInfo>();

        while (result.Count < 3)
        {
            int index = Random.Range(0, availableList.Count);
            result.Add(availableList[index]);
            availableList.RemoveAt(index);
        }

        return result;
    }

    /// <summary>
    /// UI 스크립트를 등록하고 능력 목록 UI 아이템을 생성합니다.
    /// </summary>
    /// <param name="script">UI 패널 스크립트</param>
    public void SetQueenInhanceUIController(QueenEnhanceUIController script)
    {
        queenEnhanceUIController = script;
    }

    /// <summary>
    /// 강화 항목 선택 시 실제 적용되는 함수
    /// </summary>
    public void ApplyInhance(QueenEnhanceInfo info)
    {
        int id = info.ID;

        // 레벨 증가
        if (acquiredEnhanceLevels.ContainsKey(id))
            acquiredEnhanceLevels[id]++;
        else
            acquiredEnhanceLevels[id] = 1;

        int newLevel = acquiredEnhanceLevels[id];

        Utils.Log($"{info.name} 강화 적용, 현재 레벨: {newLevel}");

        // TODO: 해당 강화 효과 적용 처리 (예: 스탯 변화 등)


    }

    public int GetEnhanceValueByID(int id)
    {
        if (!acquiredEnhanceLevels.TryGetValue(id, out int level) || level <= 0)
            return 0;

        if (!DataManager.Instance.queenEnhanceDic.TryGetValue(id, out var info))
            return 0;

        int total = 0;

        for (int i = 1; i <= level; i++)
        {
            total += info.state_Base + info.state_LevelUp * (i - 1);
        }

        return total;
    }

    public int GetEnhanceLevel(int id)
    {
        return acquiredEnhanceLevels.TryGetValue(id, out var level) ? level : 0;
    }

}
