using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TrophyPanel : MonoBehaviour
{

    public Image iconImg;
    public TextMeshProUGUI nameTxt;
    public Image checkImg;
    public Toggle toggle;

    // icon, 
    public void Init(string iconName, string name, bool isActive, ToggleGroup toggleGroup)
    {
        // icon선택
        if (iconName != null)
        {
            if (DataManager.Instance.iconAtlas.GetSprite(iconName) != null)
            {
                iconImg.sprite = DataManager.Instance.iconAtlas.GetSprite(iconName);
            }
        }

        nameTxt.text = name;

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
}
