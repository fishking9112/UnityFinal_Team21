using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;
using UnityEditor.Localization.Plugins.XLIFF.V12;

public class TrophyPanel : MonoBehaviour
{
    private TrophyInfo trophyInfo;
    public Image iconImg;
    public TextMeshProUGUI nameTxt;
    public LocalizeStringEvent nameLocalize;
    public LocalizeStringEvent descLocalize;
    public TextMeshProUGUI countTxt;
    public GameObject nonStackGroup;
    public GameObject countGroup;
    public GameObject completeGroup;
    public Image rewardImg;
    public TextMeshProUGUI rewardTxt;

    public Image checkImg;
    public Toggle toggle;
    public Button rewardBtn;


    private bool isCount = false;

    public void Awake()
    {
        rewardBtn.onClick.AddListener(() => { OnClickReward(); });
    }

    // icon, 
    public void Init(int id, string iconName, string name, string desc, int countMax, int countCur, bool isActive, ToggleGroup toggleGroup)
    {
        trophyInfo = DataManager.Instance.trophyDic[id];

        // icon선택
        if (iconName != null)
        {
            if (DataManager.Instance.iconAtlas.GetSprite(iconName) != null)
            {
                iconImg.sprite = DataManager.Instance.iconAtlas.GetSprite(iconName);
            }
        }

        // nameTxt.text = name;
        StringManager.Instance.SetString(name, nameLocalize);
        StringManager.Instance.SetString(desc, descLocalize);

        if (trophyInfo.type == TrophyType.Stack)
        {
            nonStackGroup.SetActive(false);
            countGroup.SetActive(true);
            countTxt.text = $"{countCur} / {countMax}";
        }
        else
        {
            nonStackGroup.SetActive(true);
            countGroup.SetActive(false);
        }

        if (countMax <= countCur) // 완료 버튼 활성화
        {
            nonStackGroup.SetActive(false);
            countGroup.SetActive(false);
            if (TrophyManager.Instance.IsRewardTrophy(trophyInfo.id))
            {
                completeGroup.SetActive(true);
            }
            else
            {
                completeGroup.SetActive(false);
            }
            isCount = true;
        }
        else
        {
            countGroup.SetActive(true);
            isCount = false;
        }

        if (isActive)
        {
            checkImg.gameObject.SetActive(true);
        }
        else
        {
            checkImg.gameObject.SetActive(false);
        }

        if (trophyInfo.unLockID != 0) // 돈이 아닌 해금
        {
            // 나중에 해금되는 것 만들 때 사용될 것(열쇠 아이콘)
            rewardImg.sprite = DataManager.Instance.iconAtlas.GetSprite("gameicon_tilemap-Sheet_2099");
            rewardTxt.text = "";
        }
        else
        {
            // 골드 획득
            rewardImg.sprite = DataManager.Instance.iconAtlas.GetSprite("gameicon_tilemap-Sheet_1179");
            rewardTxt.text = trophyInfo.reward.ToString();
        }
        toggle.group = toggleGroup;
    }

    public void OnClickReward()
    {
        if (!isCount)
        {
            Utils.Log("잘못된 버튼 클릭입니다");
            return;
        }

        bool isClear = TrophyManager.Instance.GetRewardTrophy(trophyInfo.id, completeGroup.transform.position);
        if (isClear) completeGroup.SetActive(true);

    }
}
