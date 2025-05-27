using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QueenAbilityUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject UIPanel;
    [SerializeField] private GameObject uiQueenAbilityPanelRoot;
    [SerializeField] private Transform contentTransform;
    public Transform ContentTransform => contentTransform;

    [Header("Buttons")]
    [SerializeField] private Button resetButton;
    [SerializeField] private Button clostButton;
    [SerializeField] private Button confirmButton;


    [Header("Description Popup")]
    [SerializeField] private RectTransform descriptionPopupUI;
    [SerializeField] private TextMeshProUGUI popupUIAbilityName;
    [SerializeField] private TextMeshProUGUI popupUIAbilityDec;
    [SerializeField] private TextMeshProUGUI popupUIAbilityCost;
    [SerializeField] private Image popupUIAbilityImage;


    private bool isFollowingMouse = false;


    /// <summary>
    /// 에디터에서 참조값을 자동으로 할당합니다.
    /// </summary>
    private void OnValidate()
    {
        if (popupUIAbilityName == null)
        {
            popupUIAbilityName = descriptionPopupUI.Find("AbilityNameText").GetComponent<TextMeshProUGUI>();
            popupUIAbilityDec = descriptionPopupUI.Find("AbilityDecText").GetComponent<TextMeshProUGUI>();
            popupUIAbilityCost = descriptionPopupUI.Find("AbilityUpgradeCostText").GetComponent<TextMeshProUGUI>();
            popupUIAbilityImage = descriptionPopupUI.Find("AbilityIcon").GetComponent<Image>();
        }

        if (uiQueenAbilityPanelRoot == null)
        {
            uiQueenAbilityPanelRoot = transform.Find("UIQueenAbilityPanelRoot").gameObject;
        }

        if (uiQueenAbilityPanelRoot.activeSelf)
        {
            uiQueenAbilityPanelRoot.SetActive(false);
        }

        if (descriptionPopupUI.gameObject.activeSelf)
        {
            uiQueenAbilityPanelRoot.SetActive(false);
        }
    }

    private void OnDisable()
    {
        UGSManager.Instance.SaveLoad.SaveAsync().Forget();
    }

    /// <summary>   
    /// 시작 시 매니저가 생성될 때까지 대기한 뒤 UI를 초기화합니다.
    /// </summary>
    private async void Start()
    {
        await UniTask.WaitUntil(() => QueenAbilityUpgradeManager.Instance != null);
        QueenAbilityUpgradeManager.Instance.SetQueenAbilityUIController(this);

        resetButton.onClick.RemoveAllListeners();
        resetButton.onClick.AddListener(OnClickResetButton);

        clostButton.onClick.AddListener(() => UIPanel.SetActive(false));
        confirmButton.onClick.AddListener(() => UIPanel.SetActive(false));

        gameObject.SetActive(true);
        uiQueenAbilityPanelRoot.SetActive(true);
        descriptionPopupUI.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (isFollowingMouse && descriptionPopupUI.gameObject.activeSelf)
        {
            descriptionPopupUI.position = Input.mousePosition;
        }
    }

    /// <summary>
    /// 능력 정보에 따라 팝업 UI를 설정하고 표시합니다.
    /// </summary>
    /// <param name="info">능력 정보</param>
    /// <param name="currentLevel">현재 업그레이드 레벨</param>
    public void SetPopupQueenAbilityInfo(QueenAbilityInfo info, int currentLevel)
    {
        popupUIAbilityName.text = info.name;
        string formattedValue = "";
        if (currentLevel <= 0)
        {
            formattedValue = info.type == StatModifierType.Percent ? "0%" : "0";
        }
        else
        {
            float value = info.levelInfo[currentLevel - 1].eff;
            formattedValue = info.type == StatModifierType.Percent
                ? $"{value * 100:F0}%"
                : $"{value}";
        }

        popupUIAbilityDec.text = info.description.Replace("n", formattedValue);
        popupUIAbilityCost.text = currentLevel >= info.maxLevel ? "MAX" : info.levelInfo[currentLevel].cost.ToString();
        popupUIAbilityImage.sprite = DataManager.Instance.iconAtlas.GetSprite(info.Icon);

        // 팝업 위치 및 마우스 추적 활성화
        descriptionPopupUI.gameObject.SetActive(true);
        descriptionPopupUI.position = Input.mousePosition;
        isFollowingMouse = true;
    }

    /// <summary>
    /// 능력 설명 팝업 UI를 숨기고 마우스 추적을 중지합니다.
    /// </summary>
    public void HidePopup()
    {
        descriptionPopupUI.gameObject.SetActive(false);
        isFollowingMouse = false;
    }

    /// <summary>
    /// 초기화 버튼 클릭 시 호출됩니다.
    /// </summary>
    private void OnClickResetButton()
    {
        UIManager.Instance.ShowPopup("알림", "모든 업그레이드 사항을 초기화 시키겠습니까?", () => QueenAbilityUpgradeManager.Instance.ResetAllAbilities(), () => { Utils.Log("취소."); });
    }
}
