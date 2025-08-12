using Cysharp.Threading.Tasks;
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
    public Button testBtn;
    public TextMeshProUGUI goldText;
    public GameObject buttonMenu;
    public GameObject uiMenu;
    public Transform BlackBackground;
    private GameObject activePanel;
    public GameObject redDot_Notification;
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
            UIManager.Instance.ShowPopup("9900035", "9900036", () => { UnityEditor.EditorApplication.isPlaying = false; }, () => { Utils.Log("취소."); });
#else
        UIManager.Instance.ShowPopup("9900035", "9900036", () => Application.Quit() , () => { Utils.Log("취소."); });
#endif
        });

        qustionGameBtn.onClick.AddListener(() => { UIManager.Instance.ShowTooltip((int)IDToolTip.MainMenu, true); });
        // 게임시작 버튼 누를 시 스타트 실행
        startButton.onClick.AddListener(OnClickGameStart);

#if UNITY_EDITOR
        testBtn.gameObject.SetActive(true);
        testBtn.onClick.AddListener(OnClickTestStart);
#endif

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
    public void GoldTextScaleUpAndDown()
    {
        if (this != null)
        {
            StopAllCoroutines(); // 중복 재생 방지
            StartCoroutine(CoGoldTextScaleUpAndDown());
        }
    }


    private IEnumerator CoGoldTextScaleUpAndDown()
    {

        Vector3 targetScale = Vector3.one * 1.5f;
        float growDuration = 0.2f;
        float shrinkDuration = 0.3f;

        // 1. 커지기
        float t = 0f;
        while (t < growDuration)
        {
            t += Time.deltaTime;
            float lerp = t / growDuration;
            goldText.transform.localScale = Vector3.Lerp(Vector3.one, targetScale, lerp);
            yield return null;
        }

        // 2. 줄어들기
        t = 0f;
        while (t < shrinkDuration)
        {
            t += Time.deltaTime;
            float lerp = t / shrinkDuration;
            goldText.transform.localScale = Vector3.Lerp(targetScale, Vector3.one, lerp);
            yield return null;
        }

        goldText.transform.localScale = Vector3.one; // 정확히 복원
        yield return null;
    }

    public void OnClickGameStart()
    {
        LogManager.Instance.LogEvent(GameLog.Contents.Funnel, (int)GameLog.FunnelType.TouchPlay);

        // TODO : 바뀐 스텟으로 시작(?)
        SceneLoadManager.Instance.LoadScene(LoadSceneEnum.GameScene).Forget();
    }
    public void OnClickTestStart()
    {
        SceneLoadManager.Instance.LoadScene(LoadSceneEnum.TestScene).Forget();

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
