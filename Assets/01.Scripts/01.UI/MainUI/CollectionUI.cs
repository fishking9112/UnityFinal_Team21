using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class CollectionUI : MonoBehaviour
{
    [Header("토글 선택 버튼들")]
    public List<Toggle> toggleList;
    public int toggleIndex = -1;

    [Header("도감 설명")]
    public Image lockImg;
    public Image descIcon;
    public TextMeshProUGUI title;
    public TextMeshProUGUI desc;

    [Header("도감 관리")]
    public Transform contentTransform; // 생성 할 위치
    public CollectionIcon iconPrefab; // 생성될 프리팹

    [Header("뒤로가기 버튼")]
    public Button closeButton;

    public Dictionary<int, CollectionIcon> allIcons = new(); // 프리팹으로 생성 된 Icon 목록들
    public Dictionary<int, CollectionIcon> monsterIcons = new();
    public Dictionary<int, CollectionIcon> queenIcons = new();
    public Dictionary<int, CollectionIcon> enhanceIcons = new();
    public Dictionary<int, CollectionIcon> activeSkillIcons = new();
    public Dictionary<int, CollectionIcon> heroIcons = new();
    public Dictionary<int, CollectionIcon> heroAbilityIcons = new();
    public List<Dictionary<int, CollectionIcon>> iconList = new();


    public void Start()
    {
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

        // 도감 설명 초기화
        title.text = "";
        desc.text = "";

        // 아이콘 초기화
        CreateIcons(DataManager.Instance.monsterDic, monsterIcons, contentTransform);
        CreateIcons(DataManager.Instance.queenAbilityDic, queenIcons, contentTransform);
        CreateIcons(DataManager.Instance.queenEnhanceDic, enhanceIcons, contentTransform);
        CreateIcons(DataManager.Instance.queenActiveSkillDic, activeSkillIcons, contentTransform);
        CreateIcons(DataManager.Instance.heroStatusDic, heroIcons, contentTransform);
        CreateIcons(DataManager.Instance.heroAbilityDic, heroAbilityIcons, contentTransform);

        iconList.Add(allIcons);
        iconList.Add(monsterIcons);
        iconList.Add(queenIcons);
        iconList.Add(enhanceIcons);
        iconList.Add(activeSkillIcons);
        iconList.Add(heroIcons);
        iconList.Add(heroAbilityIcons);

        closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    /// <summary>
    /// 메뉴 선택
    /// </summary>
    /// <param name="index"></param>
    public void SelectToggle(int index)
    {
        toggleIndex = index;
        ClearDetail();
        ShowDetail(toggleIndex);
    }

    /// <summary>
    /// 아이콘 Dictionary 형태로 생성
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="dataDic"></param>
    /// <param name="targetDic"></param>
    /// <param name="parent"></param>
    private void CreateIcons<T>(Dictionary<int, T> dataDic, Dictionary<int, CollectionIcon> targetDic, Transform parent) where T : IInfo
    {
        foreach (var pair in dataDic)
        {
            var tempPrefab = Instantiate(iconPrefab, parent);
            IInfo info = pair.Value;
            tempPrefab.Init(info, OnClickDetail);
            allIcons.Add(info.ID, tempPrefab);
            targetDic.Add(info.ID, tempPrefab);
        }
    }

    /// <summary>
    /// 아이콘 클릭 시 디테일 보여주기 allIcons들에게 Action으로 callback함수로 넘김
    /// </summary>
    public void OnClickDetail(Sprite sprite, string _name, string _description, bool _isActive)
    {
        descIcon.sprite = sprite;
        title.text = _name;
        desc.text = _description;
        if (_isActive)
        {
            lockImg.gameObject.SetActive(false);
        }
        else
        {
            lockImg.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 다른 곳 클릭 시 설명창 초기화 시키기
    /// </summary>
    public void ClearDetail()
    {
        descIcon.sprite = null;
        title.text = "";
        desc.text = "";
        lockImg.gameObject.SetActive(false);
        foreach (var icon in allIcons)
        {
            icon.Value.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 버튼 클릭 시 설명창에 설명 적어주기
    /// </summary>
    /// <param name="index"></param>
    public void ShowDetail(int index)
    {
        foreach (var icon in iconList[index])
        {
            icon.Value.gameObject.SetActive(true);
        }
    }
}
