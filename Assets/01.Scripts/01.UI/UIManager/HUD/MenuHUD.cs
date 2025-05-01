using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class MainUIButtonPanel
{
    public string name;
    public Button button;
    public GameObject panel;
}


public class MenuHUD : HUDUI
{
    public List<MainUIButtonPanel> mainUISets;
    public Button backBtn;

    public TextMeshProUGUI goldText;
    public GameObject buttonMenu;
    public GameObject uiMenu;
    public Transform BlackBackground;
    private GameObject activePanel;



    public override void Initialize()
    {
        foreach (var mainUISet in mainUISets)
        {
            mainUISet.button.onClick.AddListener(() =>
            {
                mainUISet.panel.SetActive(true);
               // buttonMenu.SetActive(false);
                uiMenu.SetActive(true);
                activePanel = mainUISet.panel;
            });
        }

        backBtn.onClick.AddListener(() =>
        {
           // buttonMenu.SetActive(true);
            uiMenu.SetActive(false);
            activePanel.SetActive(false);
        });

        // 씬에 들어 갈때 골드 업데이트
        goldText.text = Utils.GetThousandCommaText(GameManager.Instance.Gold.Value);
        GameManager.Instance.Gold.AddAction(value => RefreshGoldText(value));

        BlackBackground.SetAsFirstSibling();
        BlackBackground.gameObject.SetActive(false);
    }

    public void RefreshGoldText(int gold)
    {
        goldText.text = Utils.GetThousandCommaText(gold);
    }
}
