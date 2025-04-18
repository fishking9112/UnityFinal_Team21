using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public class Page
{
    public int index;
    public string name;
    public GameObject evolutionTree;
}

public class EvolutionWindowUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI monsterNameText;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private List<GameObject> selectedMonster;

    [SerializeField] private List<Page> pageList;

    private int curIndex;

    private void Start()
    {
        leftButton.onClick.AddListener(OnClickLeftButton);
        rightButton.onClick.AddListener(OnClickRightButton);
    }

    private void OnEnable()
    {
        curIndex = 0;
        UpdateUI();
    }

    private void OnDisable()
    {

    }

    // UI 초기화
    private void UpdateUI()
    {
        // 양쪽 맨 끝 페이지에서는 버튼 하나 비활성화
        leftButton.gameObject.SetActive(curIndex > 0);
        rightButton.gameObject.SetActive(curIndex < pageList.Count - 1);

        // 현재 페이지에 맞는 진화 트리 활성화
        for (int i = 0; i < pageList.Count; i++)
        {
            pageList[i].evolutionTree.SetActive(i == curIndex);
        }

        // 현재 페이지에 맞는 몬스터 이름 출력
        if (curIndex >= 0 && curIndex < pageList.Count)
        {
            monsterNameText.text = pageList[curIndex].name;
        }

        // 현재 페이지 순번의 다이아몬드 모양 UI가 커짐 
        for(int i = 0; i < selectedMonster.Count; i++)
        {
            if (selectedMonster[i] != null)
            {
                selectedMonster[i].transform.localScale = (i == curIndex) ? new Vector3(3.0f, 3.0f, 3.0f) : Vector3.one;
            }
        }
    }

    // 이전 페이지로 이동
    public void OnClickLeftButton()
    {
        if (curIndex > 0)
        {
            curIndex--;
            UpdateUI();
        }
    }

    // 다음 페이지로 이동
    public void OnClickRightButton()
    {
        if (curIndex < pageList.Count - 1)
        {
            curIndex++;
            UpdateUI();
        }
    }
}
