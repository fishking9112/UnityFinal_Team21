using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrophyUI : MonoBehaviour
{

    [Header("토글 선택 버튼들")]
    public ToggleGroup toggleGroup;
    public Transform parent;
    public TrophyPanel panelPrefab;

    [Header("업적 설명")]
    public List<TrophyInfo> infos = new();
    public List<Toggle> toggleList = new();

    [Header("뒤로가기 버튼")]
    public Button closeButton;

    void Start()
    {
        foreach (var pair in DataManager.Instance.trophyDic)
        {
            var tempPrefab = Instantiate(panelPrefab, parent);
            var temp = pair.Value;
            tempPrefab.Init(temp.icon, temp.name, TrophyManager.Instance.trophyClear[temp.id], toggleGroup);
            infos.Add(temp);
            toggleList.Add(tempPrefab.toggle);
        }

        // 게임 시작 버튼 초기화
        for (int i = 0; i < toggleList.Count; i++)
        {
            int index = i; // 클로저 캡처 방지용
            toggleList[i].onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    SelectToggle(index);
                }
            });
        }

        // 첫번째 무조건 선택
        toggleList[0].isOn = true;

        closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    /// <summary>
    /// 메뉴 선택
    /// </summary>
    /// <param name="index"></param>
    public void SelectToggle(int index)
    {
        OnClickDetail(infos[index]);
    }

    public void OnClickDetail(TrophyInfo trophyInfo)
    {

        string unlockCollection = "";
        if (trophyInfo.unLockID != 0 && TrophyManager.Instance.allCollections.ContainsKey(trophyInfo.unLockID))
        {
            string tempName = TrophyManager.Instance.allCollections[trophyInfo.unLockID].Name;
            unlockCollection = $"{tempName} 해금 ";
        }
        string gainGold = trophyInfo.reward != 0 ? $"{trophyInfo.reward} 골드 획득" : "";

    }

}
