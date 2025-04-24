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

public class EvolutionTreeUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI monsterNameText;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private List<GameObject> selectedMonster;

    [Header("Pages")]
    [SerializeField] private List<Page> pageList;

    public int PageCount => pageList.Count;

    public void SetButtonCallbacks(Action onLeftClick, Action onRightClick)
    {
        leftButton.onClick.RemoveAllListeners();
        rightButton.onClick.RemoveAllListeners();

        leftButton.onClick.AddListener(() => onLeftClick?.Invoke());
        rightButton.onClick.AddListener(() => onRightClick?.Invoke());
    }

    public void ShowPage(int index)
    {
        // 버튼 상태 갱신
        leftButton.gameObject.SetActive(index > 0);
        rightButton.gameObject.SetActive(index < pageList.Count - 1);

        // 트리 활성화
        for (int i = 0; i < pageList.Count; i++)
        {
            pageList[i].evolutionTree.SetActive(i == index);
        }

        // 이름 표시
        if (index >= 0 && index < pageList.Count)
        {
            monsterNameText.text = pageList[index].name;
        }

        // 아이콘 크기 조절
        for (int i = 0; i < selectedMonster.Count; i++)
        {
            if (selectedMonster[i] != null)
            {
                selectedMonster[i].transform.localScale = (i == index) ? new Vector3(3f, 3f, 3f) : Vector3.one;
            }
        }
    }
}
