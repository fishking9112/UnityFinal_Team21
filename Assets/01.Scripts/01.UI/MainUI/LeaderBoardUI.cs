using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardUI : MonoBehaviour
{
    [SerializeField] private Transform rankScrollContent;
    [SerializeField] private RankPanel myRankPanel;
    [SerializeField] private RankPanel rankPanelPrefab;
    [SerializeField] private GameObject noRankText;
    [SerializeField] private Button closeBtn;
    [SerializeField] private Button refreshBtn;
    [SerializeField] private Image refreshCoolTiemImage;

    private RankInfo myRankInfo;
    private List<RankInfo> rankerInfo;

    private const float refreshCooldown = 10f; // 쿨타임 10초
    private float cooldownTimer = 0f;
    private bool isCooldownActive = false;

    private void Awake()
    {
        noRankText.SetActive(false);
        closeBtn.onClick.AddListener(() => gameObject.SetActive(false));
        refreshBtn.onClick.AddListener(OnClickRefresh);
        refreshCoolTiemImage.fillAmount = 0f;
        refreshBtn.interactable = true;
    }

    private void OnEnable()
    {
        UpdateLeaderboardUI();
        // 쿨타임 초기화
        StartCooldown();
    }

    private void Update()
    {
        if (isCooldownActive)
        {
            cooldownTimer -= Time.deltaTime;
            refreshCoolTiemImage.fillAmount = cooldownTimer / refreshCooldown;
            Debug.Log("sfe");
            if (cooldownTimer <= 0)
            {
                StopCooldown();
            }
        }
    }

    private void OnClickRefresh()
    {
        if (isCooldownActive) return;

        RefreshLeaderboardAsync().Forget();
        StartCooldown();
    }

    private async UniTaskVoid RefreshLeaderboardAsync()
    {
        noRankText.SetActive(false);

        // 서버에서 최신 데이터 불러오기
        await UGSManager.Instance.LoadLeaderboardTop10Async();

        // UI 갱신
        UpdateLeaderboardUI();
    }

    private void UpdateLeaderboardUI()
    {
        foreach (Transform child in rankScrollContent)
        {
            Destroy(child.gameObject);
        }

        rankerInfo = UGSManager.Instance.Leaderboard.rankerInfo;
        myRankInfo = UGSManager.Instance.Leaderboard.myRankerInfo;

        noRankText.SetActive(rankerInfo.Count <= 0);

        for (int i = 0; i < rankerInfo.Count; i++)
        {
            RankPanel rankPanel = Instantiate(rankPanelPrefab, rankScrollContent);
            rankPanel.SetRankPanel(rankerInfo[i]);
        }

        myRankPanel.SetRankPanel(myRankInfo);
    }

    private void StartCooldown()
    {
        isCooldownActive = true;
        cooldownTimer = refreshCooldown;
        refreshBtn.interactable = false;
        refreshCoolTiemImage.fillAmount = 1f;
    }

    private void StopCooldown()
    {
        isCooldownActive = false;
        cooldownTimer = 0f;
        refreshBtn.interactable = true;
        refreshCoolTiemImage.fillAmount = 0f;
    }
}
