using System;
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

    public Button SelectBtn;
    public Image MainQueenImage;
    public QueenSelectItem prefabsQueenSelectItem;
    public Transform parentQueenSelectItem;
    public ToggleGroup queenSelectToggleGroup;
    public QueenBasicSkillDescription QueenBasicSkillDescription;
    public QueenBasicSkillDescription[] ArrayQueenPassiveSkillDescription;

    private void Start()
    {
        queenSelectToggleList.Clear();
        SelectBtn.onClick.AddListener(() => gameObject.SetActive(false));
        InitializeQueenItems();
        RegisterToggleEvents();

        if (queenSelectToggleList.Count > 0)
            queenSelectToggleList[0].isOn = true;
    }

    private void InitializeQueenItems()
    {
        foreach (var pair in DataManager.Instance.queenStatusDic)
        {
            int queenID = pair.Key;
            QueenSelectItem item = Instantiate(prefabsQueenSelectItem, parentQueenSelectItem);
            item.SetQueenSelectItem(queenID, queenSelectToggleGroup);
            queenSelectToggleList.Add(item.ThisToggle);
        }
    }

    private void RegisterToggleEvents()
    {
        for (int i = 0; i < queenSelectToggleList.Count; i++)
        {
            int index = i; // 클로저 문제 방지
            queenSelectToggleList[i].onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    var item = queenSelectToggleList[index].GetComponent<QueenSelectItem>();
                    SelectQueen(item.QueenID);
                }
            });
        }
    }

    public void SelectQueen(int queenID)
    {
        GameManager.Instance.QueenCharaterID = queenID;

        var queenInfo = DataManager.Instance.queenStatusDic[queenID];
        MainQueenImage.sprite = DataManager.Instance.iconAtlas.GetSprite(queenInfo.Image);

        // 액티브 스킬 설정
        var activeSkill = DataManager.Instance.queenActiveSkillDic[queenInfo.baseActiveSkill];
        SetSkillInfo(QueenBasicSkillDescription, activeSkill);

        // 패시브 스킬 설정
        int[] passiveSkillIDs = new int[]
        {
            queenInfo.basePassiveSkill_1,
            queenInfo.basePassiveSkill_2,
            queenInfo.basePassiveSkill_3
        };

        for (int i = 0; i < ArrayQueenPassiveSkillDescription.Length && i < passiveSkillIDs.Length; i++)
        {
            var passiveSkill = DataManager.Instance.queenPassiveSkillDic[passiveSkillIDs[i]];
            SetSkillInfo(ArrayQueenPassiveSkillDescription[i], passiveSkill);
        }
    }

    // QueenActiveSkillInfo에 맞는 오버로드
    private void SetSkillInfo(QueenBasicSkillDescription ui, QueenActiveSkillInfo skill)
    {
        ui.SkillIcon.sprite = DataManager.Instance.iconAtlas.GetSprite(skill.Icon);
        ui.SkillName.text = skill.Name;
        ui.SkillDescription.text = skill.Description;
    }

    // QueenPassiveSkillInfo에 맞는 오버로드
    private void SetSkillInfo(QueenBasicSkillDescription ui, QueenPassiveSkillInfo skill)
    {
        ui.SkillIcon.sprite = DataManager.Instance.iconAtlas.GetSprite(skill.Icon);
        ui.SkillName.text = skill.Name;
        ui.SkillDescription.text = skill.Description;
    }

}
