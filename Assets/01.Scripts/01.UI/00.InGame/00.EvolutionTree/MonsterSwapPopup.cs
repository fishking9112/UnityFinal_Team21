using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterSwapPopup : MonoBehaviour
{
    [SerializeField] private GameObject panelRoot;
    [SerializeField] private List<Image> slotIconList;
    [SerializeField] private List<Button> slotButtonList;
    [SerializeField] private Button closeButton;

    private EvolutionTreeUI evolutionTreeUI;
    private MonsterInfo evolvedMonster;

    public void Init(EvolutionTreeUI treeUI)
    {
        evolutionTreeUI = treeUI;
        closeButton.onClick.AddListener(ClosePopup);
        panelRoot.SetActive(false);
    }

    public void OpenPopup(MonsterInfo newMonster)
    {
        evolvedMonster = newMonster;
        panelRoot.SetActive(true);

        // 현재 슬롯 상태를 UI에 반영
        for (int i = 0; i < slotButtonList.Count; i++)
        {
            int index = i;
            var btn = slotButtonList[i];

            // UI에 현재 몬스터 표시
            EvolutionSlot currentSlot = evolutionTreeUI.SlotList[index];
            if (currentSlot.slotMonsterInfoData != null)
            {
                slotIconList[index].sprite = DataManager.Instance.iconAtlas.GetSprite(currentSlot.slotMonsterInfoData.icon);
                slotIconList[index].enabled = true;
            }
            else
            {
                slotIconList[index].sprite = null;
                slotIconList[index].enabled = false;
            }

            // 버튼 클릭 리스너 세팅
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => OnClickSlot(index));
        }
    }

    private void OnClickSlot(int index)
    {
        if (evolvedMonster == null)
        {
            return;
        }

        EvolutionSlot targetSlot = evolutionTreeUI.SlotList[index];

        // 이미 등록된 몬스터가 있으면 제거
        if (targetSlot.slotMonsterInfoData != null)
        {
            evolutionTreeUI.RemoveQueenSlot(targetSlot.slotIndex);
            targetSlot.ClearSlot();
        }

        // 새로운 몬스터 등록
        targetSlot.SetSlot(evolvedMonster);
        evolutionTreeUI.AddQueenSlot(evolvedMonster, index);

        ClosePopup();
    }

    public void ClosePopup()
    {
        panelRoot.SetActive(false);
        evolvedMonster = null;
    }
}
