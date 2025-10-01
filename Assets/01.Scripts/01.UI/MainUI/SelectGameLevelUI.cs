using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using static GameLog;


public class SelectGameLevelUI : MonoBehaviour
{
    public event Action OnComplete;

    public Button SelectBtn;
    public Button CloseBtn;

    [Header("난이도 버튼")]
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button endlessButton;
    [SerializeField] private Transform selectedUIGroup;
    [SerializeField] private LocalizeStringEvent selectedTextUI;

    [Header("설명 표시용 텍스트")]
    [SerializeField] private TextMeshProUGUI descriptionTMPText;
    [SerializeField] private LocalizeStringEvent descriptionText;

    private void Awake()
    {
        easyButton.onClick.AddListener(() => SelectLevel(GameLevel.Easy));
        normalButton.onClick.AddListener(() => SelectLevel(GameLevel.Normal));
        hardButton.onClick.AddListener(() => SelectLevel(GameLevel.Hard));
        endlessButton.onClick.AddListener(() => SelectLevel(GameLevel.Endless));
    }

    private void OnEnable()
    {
        descriptionTMPText.text = "-";
        selectedUIGroup.gameObject.SetActive(false);

        if(GameManager.Instance.SelectedLevel != GameLevel.None)
        {
            SelectLevel(GameManager.Instance.SelectedLevel);
        }
    }

    public void Init()
    {
        selectedUIGroup.gameObject.SetActive(false);
        SelectBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
            OnComplete?.Invoke();
        });
        CloseBtn.onClick.AddListener(() => gameObject.SetActive(false));
    }

    // 난이도별 설명 출력
    private void SelectLevel(GameLevel level)
    {
        selectedUIGroup.gameObject.SetActive(true);
        switch (level)
        {
            case GameLevel.Easy:
                selectedUIGroup.SetParent(easyButton.transform.parent);
                StringManager.Instance.SetString("9900069", selectedTextUI);
                StringManager.Instance.SetString("9900074", descriptionText, "-30", "+30");
                break;

            case GameLevel.Normal:
                selectedUIGroup.SetParent(normalButton.transform.parent);
                StringManager.Instance.SetString("9900070", selectedTextUI);
                StringManager.Instance.SetString("9900074", descriptionText, "-10", "+10");
                break;

            case GameLevel.Hard:
                selectedUIGroup.SetParent(hardButton.transform.parent);
                StringManager.Instance.SetString("9900071", selectedTextUI);
                StringManager.Instance.SetString("9900074", descriptionText, "+10", "-10");
                break;

            case GameLevel.Endless:
                selectedUIGroup.SetParent(endlessButton.transform.parent);
                StringManager.Instance.SetString("9900072", selectedTextUI);
                StringManager.Instance.SetString("9900075", descriptionText);
                break;
        }
        selectedUIGroup.transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        GameManager.Instance.SelectedLevel = level;
    }
}

