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

        switch (GameManager.Instance.SelectedLevel)
        {
            case GameLevel.Easy:
                foreach (var stat in heroStats)
                    stat.health *= 0.7f; // 용사 체력 -30%

                foreach (var stat in monsterStats)
                    stat.health *= 1.3f; // 몬스터 체력 +30%
                break;

            case GameLevel.Normal:
                foreach (var stat in heroStats)
                    stat.health *= 0.9f; // 용사 체력 -10%

                foreach (var stat in monsterStats)
                    stat.health *= 1.1f; // 몬스터 체력 +10%
                break;

            case GameLevel.Hard:
            case GameLevel.Endless: // 무한모드 = 어려움
                foreach (var stat in heroStats)
                    stat.health *= 1.1f; // 용사 체력 +10%

                foreach (var stat in monsterStats)
                    stat.health *= 0.9f; // 몬스터 체력 -10%
                break;
        }

        ShouldRestoreDifficulty = true;
    }

    public void ResetDifficulty()
    {
        if (!ShouldRestoreDifficulty) return;

        var heroStats = DataManager.Instance.heroStatusDic.Values;
        var monsterStats = DataManager.Instance.queenAbilityMonsterStatDic.Values;

        switch (GameManager.Instance.SelectedLevel)
        {
            case GameLevel.Easy:
                foreach (var stat in heroStats)
                    stat.health /= 0.7f;

                foreach (var stat in monsterStats)
                    stat.health /= 1.3f;
                break;

            case GameLevel.Normal:
                foreach (var stat in heroStats)
                    stat.health /= 0.9f;

                foreach (var stat in monsterStats)
                    stat.health /= 1.1f;
                break;

            case GameLevel.Hard:
            case GameLevel.Endless:
                foreach (var stat in heroStats)
                    stat.health /= 1.1f;

                foreach (var stat in monsterStats)
                    stat.health /= 0.9f;
                break;
        }

        ShouldRestoreDifficulty = false;
        SceneLoadManager.Instance.gameSceneStartFlow = null;
    }
}
