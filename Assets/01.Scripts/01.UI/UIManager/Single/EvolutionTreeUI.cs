using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Page
{
    public int index;
    public string name;
    public GameObject evolutionTree;
}

[Serializable]
public class MonsterCategoryGroup
{
    public int index;
    public string name;
    public GameObject selectedPanel;
    public Button selectBtn;
}

public class EvolutionTreeUI : SingleUI
{
    [Header("UI Components")]
    [SerializeField] private Button evolutionButton;
    [SerializeField] private List<MonsterCategoryGroup> monsterCategoryList;

    [Header("Pages")]
    [SerializeField] private List<Page> pageList;

    [Header("DescriptionUI")]
    [SerializeField] private TextMeshProUGUI evolutionPointText;
    [SerializeField] private Image descriptionImage;
    [SerializeField] private TextMeshProUGUI monsterName;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI costText;

    [Header("QuickSlot")]
    [SerializeField] private List<EvolutionSlot> slotList;

    private int curIndex;
    private EvolutionTree currentTreePage;

    public List<EvolutionSlot> SlotList => slotList;
    public int PageCount => pageList.Count;

    private void OnEnable()
    {
        curIndex = 0;
        ShowPage(curIndex);
    }

    private void Start()
    {
        SetButtonCallbacks();
    }

    private void SetButtonCallbacks()
    {
        for (int i = 0; i < monsterCategoryList.Count; i++)
        {
            int index = monsterCategoryList[i].index;

            if (monsterCategoryList[i].selectBtn != null)
            {
                monsterCategoryList[i].selectBtn.onClick.RemoveAllListeners();
                monsterCategoryList[i].selectBtn.onClick.AddListener(() => ShowPage(index));
            }
        }

        if (evolutionButton != null)
        {
            evolutionButton.onClick.RemoveAllListeners();
            evolutionButton.onClick.AddListener(OnClickEvolutionButton);
        }
    }

    public void ShowPage(int index)
    {
        // 페이지 표시
        for (int i = 0; i < pageList.Count; i++)
        {
            pageList[i].evolutionTree.SetActive(i == index);
        }

        // 이름 표시
        if (index >= 0 && index < pageList.Count)
        {
            currentTreePage = pageList[index].evolutionTree.GetComponent<EvolutionTree>();
        }

        // 해당 카테고리의 선택 표시 UI 활성화
        for (int i = 0; i < monsterCategoryList.Count; i++)
        {
            monsterCategoryList[i].selectedPanel.SetActive(i == index);
        }
    }

    // 설명창 초기화
    public void UpdateDescriptionWindow(EvolutionNode node)
    {
        MonsterInfo info = node.monsterInfo;

        descriptionImage.enabled = true;
        descriptionImage.sprite = node.image.sprite;
        description.text = info.description;
        healthText.text = $"기본 체력 : {info.health}";
        attackText.text = $"기본 공격력 : {info.attack}";
        costText.text = $"소환 비용 : {info.cost}";
    }

    public void OnClickEvolutionButton()
    {
        currentTreePage?.OnClickEvolutionButton();
    }

    public void SetEvolutionButtonState(bool state)
    {
        evolutionButton.gameObject.SetActive(state);
    }

    public void UpdateEvolutionPointText(float evolutionPoint)
    {
        evolutionPointText.text = evolutionPoint.ToString();
    }

    public void SetSlotList(EvolutionTree evolutionTree)
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            slotList[i].evolutionTree = evolutionTree;
            slotList[i].slotIndex = i;
        }
    }
}
