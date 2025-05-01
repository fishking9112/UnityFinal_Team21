using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHUD : HUDUI
{
    [Header("UI")]
    public MonsterSlot monsterSlot;
    public QueenActiveSkillSlot queenActiveSkillSlot;

    private QueenCondition condition => GameManager.Instance.queen.condition;

    [Header("레벨")]
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("골드")]
    [SerializeField] private TextMeshProUGUI goldText;

    [Header("게이지")]
    [SerializeField] private GaugeUI queenActiveSkillGaugeUI;
    [SerializeField] private GaugeUI summonGaugeUI;
    [SerializeField] private GaugeUI expGaugeUI;

    [Header("타이머")]
    // 곧 지워 질 것(?)
    [SerializeField] private TextMeshProUGUI timerText;
    private ReactiveProperty<float> curTime => GameManager.Instance.curTime;// new ReactiveProperty<float>();

    [Header("버튼")]
    [SerializeField] private Button pauseButton;

    [Header("미니맵")]
    [SerializeField] private MiniMapClick miniMap;


    [Header("InGame에 대한 UI들")]
    public QueenEnhanceUI queenEnhanceUI;
    public EvolutionTreeUI evolutionTreeUI;
    public PauseUI pauseUI;
    public GameResultUI gameResultUI;

    [Header("현재 상태")]
    public bool isPaused = false;
    [NonSerialized] public GameObject openWindow = null;

    [Header("레벨업 테스트 버튼")]
    public Button LevelUPTestButton;

    [Header("체력 UI 테스트 버튼")]
    public Button HealthUITestButton;

    public override void Initialize()
    {
        BindSlotButton();

        condition.Level.AddAction(UpdateLevelText);
        UpdateLevelText(condition.Level.Value);

        condition.Gold.AddAction(UpdateGoldText);
        UpdateGoldText(condition.Gold.Value);

        curTime.AddAction(UpdateTimerText);
        UpdateTimerText(curTime.Value);

        summonGaugeUI.Bind(condition.CurSummonGauge, condition.MaxSummonGauge);
        queenActiveSkillGaugeUI.Bind(condition.CurQueenActiveSkillGauge, condition.MaxQueenActiveSkillGauge);
        expGaugeUI.Bind(condition.CurExpGauge, condition.MaxExpGauge);

        pauseButton.onClick.AddListener(() => StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().ShowWindow<PauseUI>());

        GameManager.Instance.cameraController.miniMapRect = miniMap.transform as RectTransform;


        // 레벨업 테스트 버튼
        LevelUPTestButton.onClick.AddListener(() => GameManager.Instance.queen.condition.AdjustCurExpGauge(100));
        HealthUITestButton.onClick.AddListener(() =>
        {
            MonsterManager.Instance.OnClickHealthUITest();
            HeroManager.Instance.OnClickHealthUITest();
        });
    }



    /// <summary>
    /// 컨트롤러들 타입을 넣어서 활성화
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controller"></param>
    public void ShowWindow<T>(T controller = null) where T : MonoBehaviour
    {
        // 만약 오픈된 창이 있다면 이미 열린 창이 있다고 표시
        if (openWindow != null)
        {
            Utils.Log("이미 열려있는 창이 있습니다");
            return;
        }

        // 타입별로 분리
        if (typeof(T) == typeof(QueenEnhanceUI))
        {
            openWindow = queenEnhanceUI.gameObject;
        }
        else if (typeof(T) == typeof(EvolutionTreeUI))
        {
            // 다른 타입 처리
            openWindow = evolutionTreeUI.gameObject;
        }
        else if (typeof(T) == typeof(PauseUI))
        {
            openWindow = pauseUI.gameObject;
            GameManager.Instance.cameraController.miniMapRect = pauseUI.cameraRect;
        }
        else if (typeof(T) == typeof(GameResultUI))
        {
            openWindow = gameResultUI.gameObject;
        }
        else
        {
            Utils.Log("없는 타입의 Controller입니다.");
            return;
        }

        openWindow.SetActive(true);
        Time.timeScale = 0f; // 시간 멈춤
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideWindow()
    {
        if (openWindow == null) return;

        openWindow.SetActive(false);
        openWindow = null;
        Time.timeScale = 1f; // 시간 흐름
        isPaused = false;
        GameManager.Instance.cameraController.miniMapRect = miniMap.transform as RectTransform;
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            if (pauseUI != null)
            {
                ShowWindow(pauseUI);
            }
        }
    }


    public void UpdateLevelText(float level)
    {
        levelText.text = level.ToString();
    }

    public void UpdateGoldText(float gold)
    {
        goldText.text = Utils.GetThousandCommaText((int)gold);
    }

    public void UpdateTimerText(float time)
    {
        timerText.text = Utils.GetMMSSTime((int)time);
    }

    public void BindSlotButton()
    {
        for (int i = 0; i < monsterSlot.slotButtonList.Count; i++)
        {
            int index = i;
            monsterSlot.slotButtonList[i].onClick.AddListener(() =>
            {
                GameManager.Instance.queen.controller.OnClickSlotButton(index);
            });
        }

        for (int i = 0; i < queenActiveSkillSlot.slotButtonList.Count; i++)
        {
            int index = i;
            queenActiveSkillSlot.slotButtonList[i].onClick.AddListener(() =>
            {
                GameManager.Instance.queen.controller.OnClickSlotButton(index);
            });
        }
    }

    private void OnDestroy()
    {
        if (curTime != null)
        {
            curTime.RemoveAction(UpdateTimerText);
        }

        if (condition.Level != null)
        {
            condition.Level.RemoveAction(UpdateLevelText);
        }

        if (condition.Gold != null)
        {
            condition.Gold.RemoveAction(UpdateGoldText);
        }
    }
}