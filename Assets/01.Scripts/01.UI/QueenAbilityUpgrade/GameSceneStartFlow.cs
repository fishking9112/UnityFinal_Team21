using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameSceneStartFlow : MonoBehaviour
{
    [SerializeField] private IDMonster[] monsterInfoIds;

    void Awake()
    {
        StartCoroutine(DelayApply());

    }
    private IEnumerator DelayApply()
    {
        yield return null;
        yield return null;
        yield return null;
        yield return null;

        EquipDefaultUnitToQuickSlot();
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
