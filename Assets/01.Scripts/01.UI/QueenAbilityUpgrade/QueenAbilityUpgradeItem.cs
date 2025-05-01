using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QueenAbilityUpgradeItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Transform upgradeIndicator;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button downgradeButton;
    [SerializeField] private Image abilityIcon;

    private QueenAbilityInfo queenAbilityInfo;
    private QueenAbilityUpgradeManager manager;
    private List<Toggle> toggleList = new();

    /// <summary>
    /// 여왕 능력 정보를 바탕으로 UI를 초기화합니다.
    /// </summary>
    public void Initialize(QueenAbilityInfo info, int currentLevel)
    {
        manager = QueenAbilityUpgradeManager.Instance;
        queenAbilityInfo = info;

        nameText.text = queenAbilityInfo.name;

        upgradeButton.onClick.RemoveAllListeners(); 
        upgradeButton.onClick.AddListener(() =>
        {
            manager.TryUpgrade(queenAbilityInfo.id);
            Refresh(manager.GetLevel(queenAbilityInfo.id));
        });

        downgradeButton.onClick.RemoveAllListeners();
        downgradeButton.onClick.AddListener(() =>
        {
            manager.TryDowngrade(queenAbilityInfo.id);
            Refresh(manager.GetLevel(queenAbilityInfo.id));
        });

        toggleList.Clear();

        for (int i = 0; i < upgradeIndicator.childCount; i++)
        {
            var child = upgradeIndicator.GetChild(i).gameObject;
            bool isUpgradeableSlot = i < queenAbilityInfo.maxLevel;
            child.SetActive(isUpgradeableSlot);

            if (isUpgradeableSlot)
            {
                var toggle = child.GetComponent<Toggle>();
                if (toggle != null)
                {
                    toggleList.Add(toggle);
                    toggle.isOn = i < currentLevel;
                }
            }
        }

        abilityIcon.sprite = DataManager.Instance.iconAtlas.GetSprite(info.Icon);
    }

    /// <summary>
    /// 레벨 표기 toggle UI를 갱신합니다.
    /// </summary>
    /// <param name="currentLevel"></param>
    public void Refresh(int currentLevel)
    {
        for (int i = 0; i < toggleList.Count; i++)
        {
            toggleList[i].isOn = i < currentLevel;
        }
    }

    /// <summary>
    /// 마우스가 해당 항목 위로 올라갔을 때, 능력 설명 팝업을 표시합니다.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        manager.QueenAbilityUIController.SetPopupQueenAbilityInfo(queenAbilityInfo, manager.GetLevel(queenAbilityInfo.id));
    }

    /// <summary>
    /// 마우스가 항목에서 벗어날 때, 능력 설명 팝업을 숨깁니다.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        QueenAbilityUpgradeManager.Instance.QueenAbilityUIController.HidePopup();
    }
}
