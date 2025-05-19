using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class QueenBasicSkillDescription
{
    public Image SkillIcon;
    public TextMeshProUGUI SkillName;
    public TextMeshProUGUI SkillDescription;
}

public class QueenSelectUI : MonoBehaviour
{
    public List<Toggle> queenSelectToggleList;

    public Image MainQueenImage;
    public QueenSelectItem prefabsQueenSelectItem;
    public Transform parentQueenSelectItem;
    public ToggleGroup queenSelectToggleGroup;
    public QueenBasicSkillDescription QueenBasicSkillDescription;
    public QueenBasicSkillDescription[] ArrayQueenPassiveSkillDescription;

    public void Start()
    {
        queenSelectToggleList.Clear();

        foreach (var pair in DataManager.Instance.queenStatusDic)
        {
            int id = pair.Key;
            QueenStatusInfo info = pair.Value;

            QueenSelectItem queenSelectItem = Instantiate(prefabsQueenSelectItem, parentQueenSelectItem);
            queenSelectItem.SetQueenSelectItem(pair.Value.ID, queenSelectToggleGroup);
            queenSelectToggleList.Add(queenSelectItem.ThisToggle);
        }

        // TODO : 게임 시작 버튼 초기화
        for (int i = 0; i < queenSelectToggleList.Count; i++)
        {
            int index = i; // 클로저 캡처 방지용
            queenSelectToggleList[i].onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    SelectQueen(queenSelectToggleList[index].GetComponent<QueenSelectItem>().QueenID);
                }
            });
        }

        // 첫번째 무조건 선택
        queenSelectToggleList[0].isOn = true;
    }

    public void SelectQueen(int queenID)
    {
        GameManager.Instance.QueenCharaterID = queenID;
        MainQueenImage.sprite = DataManager.Instance.iconAtlas.GetSprite(DataManager.Instance.queenStatusDic[queenID].Image);

        // 보유 스킬
        int activeSkillID = DataManager.Instance.queenStatusDic[queenID].baseActiveSkill;
        QueenBasicSkillDescription.SkillIcon.sprite = DataManager.Instance.iconAtlas.GetSprite(DataManager.Instance.queenActiveSkillDic[activeSkillID].Icon);
        QueenBasicSkillDescription.SkillName.text = DataManager.Instance.queenActiveSkillDic[activeSkillID].Name;
        QueenBasicSkillDescription.SkillDescription.text = DataManager.Instance.queenActiveSkillDic[activeSkillID].Description;

        // 기본 지속 효과
        // int passiveSkillID_0 = DataManager.Instance.queenStatusDic[queenID].basePassiveSkill_1;
        // QueenBasicSkillDescription.SkillIcon.sprite = DataManager.Instance.iconAtlas.GetSprite(DataManager.Instance.queenActiveSkillDic[activeSkillID].Icon);
        // QueenBasicSkillDescription.SkillName.text = DataManager.Instance.queenActiveSkillDic[activeSkillID].Name;
        // QueenBasicSkillDescription.SkillDescription.text = DataManager.Instance.queenActiveSkillDic[activeSkillID].Description;
    }

}
