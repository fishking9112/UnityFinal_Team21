using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameSceneStartFlow : MonoBehaviour
{
    [SerializeField] private IDMonster[] monsterInfoIds;

    private async void Awake()
    {
        await WaitForInitComplete();
        QueenAbilityUpgradeManager.Instance.ApplyAllEffects();
        MonsterManager.Instance.InitComplete = true;

        // 약간의 프레임 딜레이 후 기본 유닛 장착
        await UniTask.DelayFrame(4);
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().canPause = true;
        EquipDefaultUnitToQuickSlot();
        RewardManager.Instance.initBatSummon();
    }

    /// <summary>
    /// Queen과 MonsterManager 초기화 완료될 때까지 대기
    /// </summary>
    private async UniTask WaitForInitComplete()
    {
        await UniTask.WaitUntil(() =>
            GameManager.Instance?.queen?.condition?.InitComplete == true);
    }

    private void EquipDefaultUnitToQuickSlot()
    {
        EvolutionTreeUI evolutionTreeUI = StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().evolutionTreeUI;
        evolutionTreeUI.SetQueenController();

        for (int i = 0; i < monsterInfoIds.Length; i++)
        {
            int index = i;

            if (DataManager.Instance.monsterDic.TryGetValue((int)monsterInfoIds[i], out var info))
            {
                MonsterInfo monsterInfo = info;
                evolutionTreeUI.SlotList[i].SetSlot(monsterInfo);
                evolutionTreeUI.AddQueenSlot(monsterInfo, index);
            }
        }
    }
}
