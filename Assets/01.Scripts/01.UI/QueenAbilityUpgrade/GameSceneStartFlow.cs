using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameLog;
using static UnityEngine.Rendering.DebugUI;

public class GameSceneStartFlow : MonoBehaviour
{
    [SerializeField] private IDMonster[] monsterInfoIds;

    public bool ShouldRestoreDifficulty { get; private set; }

    private async void Awake()
    {
        await WaitForInitComplete();
        QueenAbilityUpgradeManager.Instance.ApplyAllEffects();
        ApplyDifficulty();
        SceneLoadManager.Instance.gameSceneStartFlow = this;

        MonsterManager.Instance.InitComplete = true;
        HeroManager.Instance.GameStart();

        // MonsterSummonManager 초기화
        MonsterSummonManager.Instance.Initialize();

        // 약간의 프레임 딜레이 후 기본 유닛 장착
        await UniTask.DelayFrame(4);
        EquipDefaultUnitToQuickSlot();
        RewardManager.Instance.initBatSummon();

        await UniTask.Delay(1500);
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().canPause = true;
    }

    /// <summary>
    /// Queen과 MonsterManager 초기화 완료될 때까지 대기d
    /// </summary>
    private async UniTask WaitForInitComplete()
    {
        await UniTask.WaitUntil(() =>
            GameManager.Instance?.queen?.condition?.InitComplete == true);
            
        await UniTask.WaitUntil(() =>
            ObjectPoolManager.Instance?.InitComplete == true);
    }

    private void EquipDefaultUnitToQuickSlot()
    {
        EvolutionTreeUI evolutionTreeUI = StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().evolutionTreeUI;
        evolutionTreeUI.SetQueenController();

        for (int i = 0; i < evolutionTreeUI.SlotList.Count; i++)
        {
            if (i < monsterInfoIds.Length)
            {
                if (DataManager.Instance.monsterDic.TryGetValue((int)monsterInfoIds[i], out var info))
                {
                    MonsterInfo monsterInfo = info;
                    evolutionTreeUI.SlotList[i].SetSlot(monsterInfo);
                    evolutionTreeUI.AddQueenSlot(monsterInfo, i);
                }
                else
                {
                    evolutionTreeUI.SlotList[i].ClearSlot();
                    evolutionTreeUI.AddQueenSlot(null, i);
                }
            }
            else
            {
                evolutionTreeUI.SlotList[i].ClearSlot();
                evolutionTreeUI.AddQueenSlot(null, i);
            }
        }
    }

    private void ApplyDifficulty()
    {

        var heroStats = DataManager.Instance.heroStatusDic.Values;
        var monsterStats = DataManager.Instance.queenAbilityMonsterStatDic.Values;

        GameLevelStatInfo levelStat = GameManager.Instance.SelectedLevel switch
        {
            GameLevel.Easy => DataManager.Instance.gameLevelStatDic[(int)GameLevelStat.Easy],
            GameLevel.Normal => DataManager.Instance.gameLevelStatDic[(int)GameLevelStat.Normal],
            GameLevel.Hard => DataManager.Instance.gameLevelStatDic[(int)GameLevelStat.Hard],
            GameLevel.Endless => DataManager.Instance.gameLevelStatDic[(int)GameLevelStat.Endless],
            _ => null
        };

        if (levelStat == null)
        {
            Debug.LogError("선택된 난이도의 GameLevelStatInfo가 없습니다!");
            return;
        }

        float heroMultiplier = 1f + levelStat.heroHealthStat / 100f;
        float monsterMultiplier = 1f + levelStat.monsterHealthStat / 100f;

        foreach (var stat in heroStats)
            stat.health *= heroMultiplier;

        foreach (var stat in monsterStats)
            stat.health *= monsterMultiplier;

        ShouldRestoreDifficulty = true;
    }

    public void ResetDifficulty()
    {
        if (!ShouldRestoreDifficulty) return;

        var heroStats = DataManager.Instance.heroStatusDic.Values;
        var monsterStats = DataManager.Instance.queenAbilityMonsterStatDic.Values;

        GameLevelStatInfo levelStat = GameManager.Instance.SelectedLevel switch
        {
            GameLevel.Easy => DataManager.Instance.gameLevelStatDic[(int)GameLevelStat.Easy],
            GameLevel.Normal => DataManager.Instance.gameLevelStatDic[(int)GameLevelStat.Normal],
            GameLevel.Hard => DataManager.Instance.gameLevelStatDic[(int)GameLevelStat.Hard],
            GameLevel.Endless => DataManager.Instance.gameLevelStatDic[(int)GameLevelStat.Endless],
            _ => null
        };

        if (levelStat == null) return;

        float heroMultiplier = 1f + levelStat.heroHealthStat / 100f;
        float monsterMultiplier = 1f + levelStat.monsterHealthStat / 100f;

        foreach (var stat in heroStats)
            stat.health /= heroMultiplier;

        foreach (var stat in monsterStats)
            stat.health /= monsterMultiplier;

        ShouldRestoreDifficulty = false;
        SceneLoadManager.Instance.gameSceneStartFlow = null;
    }
}
