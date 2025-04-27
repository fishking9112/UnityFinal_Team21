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

public class EvolutionTreeUI : SingleUI
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI monsterNameText;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private List<GameObject> selectedMonster;

    [Header("Pages")]
    [SerializeField] private List<Page> pageList;

    private int curIndex;

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
        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();

        leftButton.onClick.AddListener(OnClickLeftButton);
        rightButton.onClick.AddListener(OnClickRightButton);
    }

    private void OnClickLeftButton()
    {
        if (curIndex > 0)
        {
            curIndex--;
            ShowPage(curIndex);
        }
    }

    private void OnClickRightButton()
    {
        if (curIndex < PageCount - 1)
        {
            curIndex++;
            ShowPage(curIndex);
        }
    }

    public void ShowPage(int index)
    {
        // 버튼 활성화/비활성화
        leftButton.gameObject.SetActive(index > 0);
        rightButton.gameObject.SetActive(index < PageCount - 1);

        // 페이지 표시
        for (int i = 0; i < pageList.Count; i++)
        {
            pageList[i].evolutionTree.SetActive(i == index);
        }

        // 이름 표시
        if (index >= 0 && index < pageList.Count)
        {
            monsterNameText.text = pageList[index].name;
        }

        // 선택된 몬스터 아이콘 크기 조정
        for (int i = 0; i < selectedMonster.Count; i++)
        {
            if (selectedMonster[i] != null)
            {
                selectedMonster[i].transform.localScale = (i == index) ? new Vector3(3f, 3f, 3f) : Vector3.one;
            }
        }
    }
}
