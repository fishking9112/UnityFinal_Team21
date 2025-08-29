using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.UI;


[Serializable]
public class QueenBasicSkillDescription
{
    public Image SkillIcon;
    public TextMeshProUGUI SkillName;
    public TextMeshProUGUI SkillDescription;
    public LocalizeStringEvent LocalizeSkillName;
    public LocalizeStringEvent LocalizeSkillDescription;
}

public class QueenSelectUI : MonoBehaviour
{
    public List<Toggle> queenSelectToggleList;

    public Button SelectBtn;
    public Button CloseBtn;
    public Image MainQueenImage;
    public QueenSelectItem prefabsQueenSelectItem;
    public Transform parentQueenSelectItem;
    public ToggleGroup queenSelectToggleGroup;
    public QueenBasicSkillDescription QueenBasicSkillDescription;
    public QueenBasicSkillDescription[] ArrayQueenPassiveSkillDescription;
    public TextMeshProUGUI queenNameText;

    public event Action OnComplete;

    private int selcetedQueenID;

    [Header("Localize용 텍스트")]
    [SerializeField] LocalizeStringEvent LocalizeQueenNameText;

    private void OnEnable()
    {
        for (int i = 0; i < queenSelectToggleList.Count; i++)
        {
            int index = i; // 클로저 문제 방지
            if (queenSelectToggleList[index].transform.GetComponent<QueenSelectItem>().QueenID == GameManager.Instance.QueenCharaterID)
            {
                queenSelectToggleList[index].isOn = true;
            }
            else
            {
                queenSelectToggleList[index].isOn = false;
            }
        }
    }

    public void Init()
    {
        queenSelectToggleList.Clear();
        SelectBtn.onClick.AddListener(() =>
        {
            GameManager.Instance.QueenCharaterID = selcetedQueenID;
            gameObject.SetActive(false);
            OnComplete?.Invoke();
        });

        CloseBtn.onClick.AddListener(() => gameObject.SetActive(false));
        InitializeQueenItems();
        RegisterToggleEvents();

        if (queenSelectToggleList.Count > 0)
            queenSelectToggleList[0].isOn = true;

        if (GameManager.Instance.QueenCharaterID == 0)
        {
            InitQueen(DataManager.Instance.queenStatusDic.First().Key);
        }
        else
        {
            InitQueen(GameManager.Instance.QueenCharaterID);
        }

        selcetedQueenID = GameManager.Instance.QueenCharaterID;
    }

    private void InitQueen(int queenID)
    {
        GameManager.Instance.QueenCharaterID = queenID;

        var queenInfo = DataManager.Instance.queenStatusDic[queenID];

        StringManager.Instance.SetString(queenInfo.Name, LocalizeQueenNameText);

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
        selcetedQueenID = queenID;

        var queenInfo = DataManager.Instance.queenStatusDic[queenID];

        StringManager.Instance.SetString(queenInfo.Name, LocalizeQueenNameText);

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

        StringManager.Instance.SetString(skill.Name, ui.LocalizeSkillName);
        StringManager.Instance.SetString(skill.Description, ui.LocalizeSkillDescription);
    }

    // QueenPassiveSkillInfo에 맞는 오버로드
    private void SetSkillInfo(QueenBasicSkillDescription ui, QueenPassiveSkillInfo skill)
    {
        ui.SkillIcon.sprite = DataManager.Instance.iconAtlas.GetSprite(skill.Icon);

        StringManager.Instance.SetString(skill.Name, ui.LocalizeSkillName);

        float percentValue = skill.value * 100f;
        StringManager.Instance.SetString(skill.Description, ui.LocalizeSkillDescription, percentValue.ToString("0"));
    }
}
