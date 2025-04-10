using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


[Serializable]
public class MainUIButtonPanel
{
    public string name;
    public Button button;
    public GameObject panel;
}


public class MainUI : MonoBehaviour
{
    public GameObject buttonMenu;
    public List<MainUIButtonPanel> mainUISets;

    public GameObject uiMenu;
    public Button backBtn;

    public GameObject goldPannel;
    public GameObject upgradePointPannel;

    private GameObject goActive;


    void Start()
    {
        foreach (var mainUISet in mainUISets)
        {
            mainUISet.button.onClick.AddListener(() =>
            {
                mainUISet.panel.SetActive(true);
                buttonMenu.SetActive(false);
                uiMenu.SetActive(true);
                goActive = mainUISet.panel;

                if (mainUISet.panel.name == "MonsterUpgradeUI") // 추후 수정 필요
                {
                    goldPannel.SetActive(false);
                    upgradePointPannel.SetActive(true);
                }
            });

            backBtn.onClick.AddListener(() =>
            {
                goActive.SetActive(false);
                buttonMenu.SetActive(true);
                uiMenu.SetActive(false);

                if (goActive.name == "MonsterUpgradeUI") // 추후 수정 필요
                {
                    goldPannel.SetActive(true);
                    upgradePointPannel.SetActive(false);
                }
            });
        }
    }


    public void MoveScene()
    {
        SceneManager.LoadScene("InGameTestScene");
    }
}
