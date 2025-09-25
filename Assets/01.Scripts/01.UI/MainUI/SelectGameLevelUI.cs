using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;
using static GameLog;

public class SelectGameLevelUI : MonoBehaviour
{
    public event Action OnComplete;

    [Header("난이도 버튼")]
    [SerializeField] private Button easyButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button hardButton;
    [SerializeField] private Button endlessButton;
    [SerializeField] private Transform selectedUIGroup;
    [SerializeField] private LocalizeStringEvent selectedTextUI;

    [Header("설명 표시용 텍스트")]
    [SerializeField] private TextMeshProUGUI descriptionText;

    private void Awake()
    {
        easyButton.onClick.AddListener(() => SelectLevel(GameLevel.Easy));
        normalButton.onClick.AddListener(() => SelectLevel(GameLevel.Normal));
        hardButton.onClick.AddListener(() => SelectLevel(GameLevel.Hard));
        endlessButton.onClick.AddListener(() => SelectLevel(GameLevel.Endless));
    }

    private void OnEnable()
    {
        descriptionText.text = "-";
        selectedUIGroup.gameObject.SetActive(false);
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
                descriptionText.text = "쉬움 - 용사 체력 -30%, 몬스터 체력 +30%";
                break;

            case GameLevel.Normal:
                selectedUIGroup.SetParent(normalButton.transform.parent);
                StringManager.Instance.SetString("9900070", selectedTextUI);
                descriptionText.text = "보통 - 용사 체력 -10%, 몬스터 체력 +10%";
                break;

            case GameLevel.Hard:
                selectedUIGroup.SetParent(hardButton.transform.parent);
                StringManager.Instance.SetString("9900071", selectedTextUI);
                descriptionText.text = "어려움 - 용사 체력 +10%, 몬스터 체력 -10%";
                break;

            case GameLevel.Endless:
                selectedUIGroup.SetParent(endlessButton.transform.parent);
                StringManager.Instance.SetString("9900072", selectedTextUI);
                descriptionText.text = "무한모드 - 어려움과 동일, 끝없이 진행";
                break;
        }

        GameManager.Instance.SelectedLevel = level;
        gameObject.SetActive(false);
        OnComplete?.Invoke();
    }
}

