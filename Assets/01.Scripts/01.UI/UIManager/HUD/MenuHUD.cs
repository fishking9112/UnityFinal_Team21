using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MainUIButtonPanel
{
    public string name;
    public Button imageButton;
    public Button panelButton;
    public GameObject panel;
}


public class MenuHUD : HUDUI
{
    public List<MainUIButtonPanel> mainUISets;
    public Button startButton;
    public Button quitGameBtn;
    public Button qustionGameBtn;
    public TextMeshProUGUI goldText;
    public GameObject buttonMenu;
    public GameObject uiMenu;
    public Transform BlackBackground;
    private GameObject activePanel;

    public QueenSelectUI queenSelectUI;

    private void Update()
    {
        // ESC 키를 누르면 모든 팝업이 닫힘
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePanel();
        }
    }

    public override async UniTask Initialize()
    {
        await UniTask.Yield(PlayerLoopTiming.Update);

        foreach (var mainUISet in mainUISets)
        {
            var panel = mainUISet.panel;

            if (mainUISet.imageButton != null)
            {
                mainUISet.imageButton.onClick.AddListener(() =>
                {
                    SetActivePanel(panel);
                });
            }

            if (mainUISet.panelButton != null)
            {
                mainUISet.panelButton.onClick.AddListener(() =>
                {
                    SetActivePanel(panel);
                });
            }

            mainUISet.panel.SetActive(false);
        }

        quitGameBtn.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UIManager.Instance.ShowPopup("게임 종료", "정말로 게임을 종료하시겠습니까?", () => { UnityEditor.EditorApplication.isPlaying = false; }, () => { Utils.Log("취소."); });
#else
        UIManager.Instance.ShowPopup("게임 종료", "정말로 게임을 종료하시겠습니까?", () => Application.Quit() , () => { Utils.Log("취소."); });
#endif
        });

        qustionGameBtn.onClick.AddListener(() => { UIManager.Instance.ShowTooltip((int)IDToolTip.MainMenu, true); });
        // 게임시작 버튼 누를 시 스타트 실행
        startButton.onClick.AddListener(OnClickGameStart);

        // 씬에 들어 갈때 골드 업데이트
        goldText.text = Utils.GetThousandCommaText(GameManager.Instance.Gold.Value);
        GameManager.Instance.Gold.AddAction(value => RefreshGoldText(value));

        BlackBackground.SetAsFirstSibling();
        BlackBackground.gameObject.SetActive(false);

        queenSelectUI.Init();
    }
    private void SetActivePanel(GameObject panel)
    {
        panel.SetActive(true);
        activePanel = panel;
    }

    public void RefreshGoldText(int gold)
    {
        goldText.text = Utils.GetThousandCommaText(gold);
    }

    public void OnClickGameStart()
    {
        LogManager.Instance.LogEvent(GameLog.Contents.Funnel, (int)GameLog.FunnelType.TouchPlay);

        // TODO : 바뀐 스텟으로 시작(?)
        SceneLoadManager.Instance.LoadScene(LoadSceneEnum.GameScene).Forget();
    }

    // 모든 창 닫기
    private void ClosePanel()
    {
        if (activePanel != null && activePanel.activeSelf)
        {
            activePanel.SetActive(false);
            activePanel = null;
        }

        activePanel = null;
    }
}
