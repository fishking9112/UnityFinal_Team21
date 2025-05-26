using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScheduledEvent
{
    public EventInfo eventInfo;
    public float triggerProgress;
    public Image icon;

    public ScheduledEvent(EventInfo info, float progress, Image iconObj)
    {
        eventInfo = info;
        triggerProgress = progress;
        icon = iconObj;
    }
}

public class GameEventProgressUI : MonoBehaviour
{
    [SerializeField] private Image eventIconPrefab;
    [SerializeField] private Image progressBarFill; // fill 이미지
    [SerializeField] private RectTransform background; // fill 백그라운드 이미지

    private readonly Queue<ScheduledEvent> scheduledEvents = new();
    private readonly List<GameEventBase> activeEvents = new();

    public Vector2 currentEventPosition = new();

    private bool isPaused => StaticUIManager.Instance.hudLayer.GetHUD<GameHUD>().isPaused;
    private bool isTimeOver => GameManager.Instance.isTimeOver;
    private float totalTime => GameManager.Instance.gameLimitTime;
    private float curTime => GameManager.Instance.curTime.Value;

    [Header("이벤트 Context를 위한 프리팹")]
    public Transform contextTransform;
    public GameEventContextUI contextUIPrefab;
    public EventMarkIcon eventMarkIconPrefab;

    [Header("이벤트 생성을 위한 프리팹")]
    public MiniCastle miniCastlePrefab;
    public MiniBarrack miniBarrackPrefab;

    private void Start()
    {
        CreateEventId(eventId: 2000001, fillPosition: 1 / 1800f); // 임시로 1초만에 퀘스트 출현
        CreateEvent(rank: 1, fillPosition: 1 / 4f);
        CreateEvent(rank: 2, fillPosition: 2 / 4f);
        CreateEvent(rank: 3, fillPosition: 3 / 4f);
    }

    private void Update()
    {
        if (isPaused || isTimeOver)
            return;

        // 프로그래스 바 진행도
        float fillAmount = (1 - (curTime / totalTime));
        progressBarFill.fillAmount = fillAmount;

        CheckEvent(fillAmount);

        // 모든 활성 이벤트 업데이트 (리스트에서 제거되도 오류 안나도록 역순)
        for (int i = activeEvents.Count - 1; i >= 0; i--)
        {
            var gameEvent = activeEvents[i];

            if (!gameEvent.IsCompleted && !gameEvent.IsFailed)
            {
                gameEvent.UpdateEvent();
            }
            else
            {
                // 완료되거나 실패한 이벤트는 리스트에서 제거 
                activeEvents.RemoveAt(i);
            }
        }
    }


    /// <summary>
    /// 이벤트 체크
    /// </summary>
    /// <param name="currentProgress"></param>
    private void CheckEvent(float currentProgress)
    {
        while (scheduledEvents.Count > 0 && scheduledEvents.Peek().triggerProgress <= currentProgress)
        {
            var nextEvent = scheduledEvents.Dequeue();
            Destroy(nextEvent.icon);
            RunEvent(nextEvent.eventInfo);
        }
    }

    /// <summary>
    /// 이벤트 실행
    /// </summary>
    /// <param name="eventTableInfo"></param>
    private void RunEvent(EventInfo eventTableInfo)
    {
        Utils.Log($"{eventTableInfo.ID} 이벤트 실행!");

        GameEventBase eventInstance = null;

        Vector2 spawnPos = Vector2.zero; // 원하는 위치
        var contextUI = Instantiate(contextUIPrefab, contextTransform);

        switch (eventTableInfo.type)
        {
            case EventTableType.Type_1: // 임의의 적 소환
                spawnPos = SpawnPointManager.Instance.heroPoint.GetRandomPosition();// GetRandomEdgePosition(98, 98); // 원하는 위치
                var heros = HeroManager.Instance.SummonHeros(spawnPos, eventTableInfo.count);
                eventInstance = new KillEnemiesEvent(heros, eventTableInfo, contextUI);
                break;

            case EventTableType.Type_2:
                // 영웅 소환
                spawnPos = SpawnPointManager.Instance.heroPoint.GetRandomPosition(); // 원하는 위치
                var bossList = DataManager.Instance.heroStatusDic
                                .Where(stat => stat.Value.heroType == HeroType.BOSS)
                                .Select(stat => stat.Value)
                                .ToList();
                var bossHero = HeroManager.Instance.SummonBoss(spawnPos, bossList[Random.Range(0, bossList.Count)].id);//eventTableInfo.createId);
                eventInstance = new KillEnemiesEvent(bossHero, eventTableInfo, contextUI);
                break;

            case EventTableType.Type_3: // 성 방어 이벤트
                spawnPos = SpawnPointManager.Instance.MiniCastlePoint.GetRandomPosition(true); // 원하는 위치
                MiniCastle castlePrefab = Instantiate(miniCastlePrefab);
                castlePrefab.transform.position = spawnPos;

                eventInstance = new DefendAreaEvent(castlePrefab, spawnPos, eventTableInfo.timer, eventTableInfo, contextUI);
                break;

            case EventTableType.Type_4: // 배럭 공격 이벤트
                spawnPos = SpawnPointManager.Instance.BarrackPoint.GetRandomPosition(true); // 원하는 위치
                MiniBarrack barrackPrefab = Instantiate(miniBarrackPrefab);
                barrackPrefab.transform.position = spawnPos;

                eventInstance = new AttackAreaEvent(barrackPrefab, spawnPos, eventTableInfo.spawnDuration, eventTableInfo, contextUI);
                break;
        }

        Instantiate(eventMarkIconPrefab, spawnPos, Quaternion.identity);
        currentEventPosition = spawnPos;

        if (eventInstance != null)
        {
            RegisterEvent(eventInstance);
        }
    }

    /// <summary>
    /// 이벤트 생성
    /// </summary>
    /// <param name="rank"></param>
    /// <param name="fillPosition"></param>
    public void CreateEvent(int rank, float fillPosition)
    {
        var candidates = DataManager.Instance.eventDic.Values.Where(x => x.rank == rank).ToList();
        if (candidates.Count == 0) return;

        var selected = candidates[Random.Range(0, candidates.Count)];

        Image icon = Instantiate(eventIconPrefab, background);
        if (selected.icon != null && selected.icon != "")
        {
            icon.sprite = DataManager.Instance.iconAtlas.GetSprite(selected.icon);
        }
        Vector2 spawnPos = GetIconSpawnPosition(fillPosition);
        icon.GetComponent<RectTransform>().anchoredPosition = spawnPos;

        scheduledEvents.Enqueue(new ScheduledEvent(selected, fillPosition, icon));
    }

    /// <summary>
    /// 이벤트 생성
    /// </summary>
    /// <param name="rank"></param>
    /// <param name="fillPosition"></param>
    public void CreateEventId(int eventId, float fillPosition)
    {
        var candidates = DataManager.Instance.eventDic.Values.Where(x => x.id == eventId).ToList();
        if (candidates.Count == 0) return;

        var selected = candidates[Random.Range(0, candidates.Count)];

        Image icon = Instantiate(eventIconPrefab, background);
        if (selected.icon != null && selected.icon != "")
        {
            icon.sprite = DataManager.Instance.iconAtlas.GetSprite(selected.icon);
        }
        Vector2 spawnPos = GetIconSpawnPosition(fillPosition);
        icon.GetComponent<RectTransform>().anchoredPosition = spawnPos;

        scheduledEvents.Enqueue(new ScheduledEvent(selected, fillPosition, icon));
    }

    /// <summary>
    /// 아이콘 나타낼 곳 설정
    /// </summary>
    /// <param name="fillAmount"></param>
    /// <returns></returns>
    private Vector2 GetIconSpawnPosition(float fillAmount)
    {
        float width = background.rect.width;
        float x = (fillAmount - 0.5f) * width;
        return new Vector2(x, 0f);
    }

    /// <summary>
    /// 이벤트 등록
    /// </summary>
    /// <param name="newEvent"></param>
    public void RegisterEvent(GameEventBase newEvent)
    {
        activeEvents.Add(newEvent);
        newEvent.StartEvent();
    }

    /// <summary>
    /// 카메라를 이벤트 방향으로 이동
    /// </summary>
    public void CameraMoveEvent()
    {
        GameManager.Instance.cameraController.MiniMapCameraMove(currentEventPosition);
    }

    public Vector2 GetRandomEdgePosition(float width, float height, float edge = 5f)
    {
        int edgeType = Random.Range(0, 4);
        float halfW = width * 0.5f;
        float halfH = height * 0.5f;

        float x = 0f, z = 0f;

        if (edgeType < 2)
        {
            x = Random.Range(-halfW, halfW);
            z = (edgeType == 0)
                ? Random.Range(halfH - edge, halfH)
                : Random.Range(-halfH, -halfH + edge);
        }
        else
        {
            z = Random.Range(-halfH, halfH);
            x = (edgeType == 2)
                ? Random.Range(-halfW, -halfW + edge)
                : Random.Range(halfW - edge, halfW);
        }

        return new Vector2(x, z);
    }

    public IEnumerable<ScheduledEvent> GetScheduledEvents()
    {
        return scheduledEvents.ToArray();
    }
    public float GetProgressAmount()
    {
        return progressBarFill.fillAmount;
    }

    public bool IsEventScheduled(ScheduledEvent evt)
    {
        return scheduledEvents.Contains(evt);
    }
}
