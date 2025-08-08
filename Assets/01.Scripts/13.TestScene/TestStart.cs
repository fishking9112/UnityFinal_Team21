using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;

public class TestStart : MonoBehaviour
{

    //public Dictionary<int, MonsterInfo> originMonsterStatDic = new Dictionary<int, MonsterInfo>();

    private async void Awake()
    {
        await WaitForInitComplete();
        QueenAbilityUpgradeManager.Instance.Initialize();
        MonsterManager.Instance.InitComplete = true;

        // 약간의 프레임 딜레이 후 기본 유닛 장착
        await UniTask.DelayFrame(4);
        EvolutionTreeUI evolutionTreeUI = StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().evolutionTreeUI;
        evolutionTreeUI.SetQueenController();

        await UniTask.Delay(1500);
        StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().canPause = true;

    }

    /// <summary>
    /// Queen과 MonsterManager 초기화 완료될 때까지 대기
    /// </summary>
    private async UniTask WaitForInitComplete()
    {
        await UniTask.WaitUntil(() =>
            GameManager.Instance?.queen?.condition?.InitComplete == true);

        await UniTask.WaitUntil(() =>
            ObjectPoolManager.Instance?.InitComplete == true);
    }

    public void ResetAllMonsterStats()
    {
        foreach (var kvp in DataManager.Instance.monsterDic)
        {
            MonsterInfo info = new MonsterInfo(kvp.Value);
            DataManager.Instance.queenAbilityMonsterStatDic[kvp.Key] = info;
        }
    }
}
