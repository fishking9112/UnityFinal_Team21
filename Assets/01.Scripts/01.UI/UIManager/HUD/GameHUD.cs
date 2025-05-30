using Cysharp.Threading.Tasks;
using System;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameHUD : HUDUI
{
    [Header("UI")]
    public MonsterSlot monsterSlot;
    public QueenActiveSkillSlot queenActiveSkillSlot;

    private QueenCondition condition => GameManager.Instance.queen.condition;

    [Header("레벨 / 골드 / 킬카운트 / 인구수")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI killCntText;
    [SerializeField] private TextMeshProUGUI populationText;

    [Header("게이지")]
    [SerializeField] private GaugeUI queenActiveSkillGaugeUI;
    [SerializeField] private GaugeUI summonGaugeUI;
    [SerializeField] private GaugeUI castleGaugeUI;
    [SerializeField] private GaugeUI expGaugeUI;
    [SerializeField] private GaugeUI queenActiveSkillGaugeTextUI; // 스킬 마나 현재량 표기
    [SerializeField] private GaugeUI summonGaugeTextUI; // 소환게이지 현재량 표기

    [Header("타이머")]
    // 곧 지워 질 것(?)
    [SerializeField] private TextMeshProUGUI timerText;
    private ReactiveProperty<float> curTime => GameManager.Instance.curTime;

    private InputAction inputAction;

    [Header("미니맵")]
    [SerializeField] private MiniMapClick miniMap;

    [Header("InGame에 대한 UI들")]
    public QueenEnhanceUI queenEnhanceUI;
    public EvolutionTreeUI evolutionTreeUI;
    public PauseUI pauseUI;
    public GameResultUI gameResultUI;

    [Header("기타 UI 오브젝트")]
    public GameObject HUDGroup;
    public GameObject BackgroundGroup;
    public GameObject TopButtonGroup;
    public GameObject EtcUIGroup;
    public GameObject OptionUIGroup;
    public GameObject EvolutionSelectUI;
    public GameObject PauseSelectUI;

    [Header("버튼 오브젝트")]
    public Button OptionBtn;
    public Button ExitBtn;
    public Button EvolutionBtn;
    public Button PauseBtn;
    public Button HudEvolutionBtn;
    public Button HudPauseBtn;
    public Button HealthUITestButton;
    public Button InGameToolTipButton;
    public Button EvolutionToolTipButton;

    [Header("현재 상태")]
    public bool isPaused = false;
    public bool canPause = false;
    [NonSerialized] public GameObject openWindow = null;

    [Header("레벨업 테스트 버튼")]
    public Button LevelUPTestButton;

    [Header("Slot")]
    public SlotChange slot;

    [Header("피격 당함 마크")]
    public AttackMarkIcon attackMarkIconPrefab;


    // 인구수용 변수
    private readonly string colorPrefixNormal = "<color=#FFFFFF>";
    private readonly string colorPrefixFull = "<color=#FF0000>";
    private readonly string colorSuffix = "</color>";

    private System.Text.StringBuilder sb = new System.Text.StringBuilder(32);

    public override async UniTask Initialize()
    {
        BindSlotButton();

        // null이 아닐 때 까지 기다림
        await UniTask.WaitUntil(() => condition != null);

        condition.Level.AddAction(UpdateLevelText);
        UpdateLevelText(condition.Level.Value);

        condition.Gold.AddAction(UpdateGoldText);
        UpdateGoldText(condition.Gold.Value);

        condition.KillCnt.AddAction(UpdateKullCntText);
        UpdateKullCntText(condition.KillCnt.Value);

        curTime.AddAction(UpdateTimerText);
        UpdateTimerText(curTime.Value);

        summonGaugeUI.Bind(condition.CurSummonGauge, condition.MaxSummonGauge);
        queenActiveSkillGaugeUI.Bind(condition.CurQueenActiveSkillGauge, condition.MaxQueenActiveSkillGauge);
        castleGaugeUI.Bind(GameManager.Instance.castle.condition.CurCastleHealth, GameManager.Instance.castle.condition.MaxCastleHealth,
        isImgFlash: true, flashAction: () =>
        {
            Instantiate(attackMarkIconPrefab, Vector2.zero, Quaternion.identity);
        });
        expGaugeUI.Bind(condition.CurExpGauge, condition.MaxExpGauge);

        queenActiveSkillGaugeTextUI.BindText(condition.CurQueenActiveSkillGauge, condition.MaxQueenActiveSkillGauge);
        summonGaugeTextUI.BindText(condition.CurSummonGauge, condition.MaxSummonGauge);

        GameManager.Instance.cameraController.miniMapRect = miniMap.transform as RectTransform;

        // 레벨업 테스트 버튼
        LevelUPTestButton.onClick.AddListener(() => GameManager.Instance.queen.condition.AdjustCurExpGauge(100));
        HealthUITestButton.onClick.AddListener(() =>
        {
            MonsterManager.Instance.OnClickHealthUITest();
            HeroManager.Instance.OnClickHealthUITest();
        });

        InGameToolTipButton.onClick.AddListener(() => UIManager.Instance.ShowTooltip((int)IDToolTip.InGame));
        EvolutionToolTipButton.onClick.AddListener(() => UIManager.Instance.ShowTooltip((int)IDToolTip.Evolution, isOnlyPage: true));
        // 옵션창 버튼 이벤트 연결
        OptionBtn.onClick.AddListener(() => ShowWindow<OptionController>());

        // Exit 버튼 이벤트 연결 
        ExitBtn.onClick.AddListener(HideWindow);

        // 진화트리 버튼 이벤트 연결 
        EvolutionBtn.onClick.AddListener(ShowEvolutionTreeUI);
        HudEvolutionBtn.onClick.AddListener(ShowEvolutionTreeUI);

        // 일시정지 버튼 이벤트 연결 
        PauseBtn.onClick.AddListener(ShowPauseUI);
        HudPauseBtn.onClick.AddListener(ShowPauseUI);

        inputAction = GameManager.Instance.queen.input.actions["PauseUI"];
        inputAction.started += OnPauseUI;

        slot.Init(GameManager.Instance.queen.controller, GameManager.Instance.queen.input.actions["SlotChange"]);

        queenEnhanceUI.gameObject.SetActive(false);
        evolutionTreeUI.gameObject.SetActive(false);
        pauseUI.gameObject.SetActive(false);
        gameResultUI.gameObject.SetActive(false);
    }

    /// <summary>
    /// 컨트롤러들 타입을 넣어서 활성화
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="controller"></param>
    public void ShowWindow<T>(T controller = null, bool isTimeOverOpen = false) where T : MonoBehaviour
    {
        if (typeof(T) != typeof(OptionController))
        {
            // 이미 창이 열려있다면 리턴
            if (openWindow != null)
            {
                Utils.Log("이미 열려있는 창이 있습니다");
                return;
            }

            // 이미 창이 열려있다면 리턴
            if (UIManager.Instance.IsOpenUI("ToolTipUI"))
            {
                Utils.Log("이미 열려있는 툴팁이 있습니다");
                return;
            }

            // 이미 창이 열려있다면 리턴
            if (GameManager.Instance.isTimeOver && !isTimeOverOpen)
            {
                Utils.Log("게임이 종료되어 기다리는 중입니다");
                return;
            }

            // 기타 UI 숨기기
            HUDGroup.SetActive(false);
            BackgroundGroup.SetActive(false);
            TopButtonGroup.SetActive(false);
            EtcUIGroup.SetActive(false);
            EvolutionSelectUI.SetActive(false);
            PauseSelectUI.SetActive(false);
        }

        // 타입별로 분리
        if (typeof(T) == typeof(QueenEnhanceUI))
        {
            openWindow = queenEnhanceUI.gameObject;
            HUDGroup.SetActive(true);
            BackgroundGroup.SetActive(true);
        }
        else if (typeof(T) == typeof(EvolutionTreeUI))
        {
            // 다른 타입 처리
            openWindow = evolutionTreeUI.gameObject;
            EvolutionSelectUI.SetActive(true);
            TopButtonGroup.SetActive(true);
            EtcUIGroup.SetActive(true);
            BackgroundGroup.SetActive(true);
        }
        else if (typeof(T) == typeof(PauseUI))
        {
            openWindow = pauseUI.gameObject;
            GameManager.Instance.cameraController.miniMapRect = pauseUI.cameraRect;
            PauseSelectUI.SetActive(true);
            TopButtonGroup.SetActive(true);
            EtcUIGroup.SetActive(true);
            BackgroundGroup.SetActive(true);
        }
        else if (typeof(T) == typeof(GameResultUI))
        {
            openWindow = gameResultUI.gameObject;
        }
        else if (typeof(T) == typeof(OptionController))
        {
            OptionUIGroup.SetActive(true);
            return;
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

        // 이미 창이 열려있다면 리턴
        if (UIManager.Instance.IsOpenUI("ToolTipUI"))
        {
            Utils.Log("이미 열려있는 툴팁이 있습니다");
            return;
        }

        HUDGroup.SetActive(true);
        BackgroundGroup.SetActive(false);
        TopButtonGroup.SetActive(false);
        EtcUIGroup.SetActive(false);
        OptionUIGroup.SetActive(false);

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

    public void UpdateKullCntText(int kullCnt)
    {
        killCntText.text = Utils.GetThousandCommaText(kullCnt);
    }

    public void UpdatePopulationText(int curPop, float maxPop)
    {
        int maxPopInt = Mathf.FloorToInt(maxPop);

        sb.Clear();
        if (curPop < maxPopInt)
        {
            sb.Append(colorPrefixNormal);
        }
        else
        {
            sb.Append(colorPrefixFull);
        }

        sb.Append(curPop);
        sb.Append(" / ");
        sb.Append(maxPopInt);
        sb.Append(colorSuffix);

        populationText.text = sb.ToString();
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
                GameManager.Instance.queen.controller.OnClickSlotButton(index, QueenSlot.MONSTER);
            });
        }

        for (int i = 0; i < queenActiveSkillSlot.slotButtonList.Count; i++)
        {
            int index = i;
            queenActiveSkillSlot.slotButtonList[i].onClick.AddListener(() =>
            {
                GameManager.Instance.queen.controller.OnClickSlotButton(index, QueenSlot.QueenActiveSkill);
            });
        }
    }

    private void ShowEvolutionTreeUI()
    {
        HideWindow();
        ShowWindow<EvolutionTreeUI>();
    }

    private void ShowPauseUI()
    {
        HideWindow();
        ShowWindow<PauseUI>();
    }

    public void OnPauseUI(InputAction.CallbackContext context)
    {
        if (!canPause) return;

        if (context.phase == InputActionPhase.Started)
        {
            if (!ReferenceEquals(openWindow, pauseUI.gameObject))
                ShowWindow<PauseUI>();
            else
                HideWindow();
        }
    }

    private void OnDestroy()
    {
        curTime?.RemoveAction(UpdateTimerText);
        condition.Level?.RemoveAction(UpdateLevelText);
        condition.Gold?.RemoveAction(UpdateGoldText);

        if (inputAction != null)
            inputAction.started -= OnPauseUI;
    }
}