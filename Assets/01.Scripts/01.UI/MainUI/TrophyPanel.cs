using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;

public class TrophyPanel : MonoBehaviour
{
    public int trophyId;
    public Image iconImg;
    public TextMeshProUGUI nameTxt;
    public LocalizeStringEvent nameLocalize;
    public LocalizeStringEvent descLocalize;
    public TextMeshProUGUI countTxt;
    public GameObject countGroup;
    public Image checkImg;
    public Toggle toggle;

    private bool isCount = false;

    // icon, 
    public void Init(int id, string iconName, string name, string desc, int countMax, int countCur, bool isActive, ToggleGroup toggleGroup)
    {
        trophyId = id;

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
        countTxt.text = $"{countCur} / {countMax}";

        if (countMax <= countCur)
        {
            countGroup.SetActive(false);
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

        toggle.group = toggleGroup;
    }

    public void OnClickReward()
    {
        if (!isCount)
        {
            Utils.Log("잘못된 버튼 클릭입니다");
            return;
        }

        bool isClear = TrophyManager.Instance.GetRewardTrophy(trophyId);
        if (isClear) countGroup.SetActive(false);
    }
}
