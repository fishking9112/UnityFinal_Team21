using Cysharp.Threading.Tasks;
using Stove.PCSDK.NET;
using System.Diagnostics;
using System.Text;
using TMPro;

public class StoveGameServiceManager : MonoSingleton<StoveGameServiceManager>
{
    public StoveAuth Auth { get; private set; }
    public StoveSaveLoad SaveLoad { get; private set; }
    public StoveLeaderboard Leaderboard { get; private set; }
    public StoveAchievement Achievement { get; private set; }

    public TextMeshProUGUI UIDtext;
    public TextMeshProUGUI statdtext;
    public event System.Action OnRequireNickname;

    private UniTaskCompletionSource<bool> nicknameRegisterTCS;

    public bool IsLoaded;

    protected override void Awake()
    {
        base.Awake();

        Auth = GetComponentInChildren<StoveAuth>();
        SaveLoad = GetComponentInChildren<StoveSaveLoad>();
        Leaderboard = GetComponentInChildren<StoveLeaderboard>();
        Achievement = GetComponentInChildren<StoveAchievement>();

        if (Achievement == null || SaveLoad == null || Leaderboard == null)
        {
            Utils.Log("Stove 관련 스크립트 누락");
        }
    }

    public void UIDtextUneable()
    {
        UIDtext.transform.parent.gameObject.SetActive(false);
    }

    public async UniTask InitAsync()
    {
        UIDtext.text = "";
        await StartGameFlowAsync();
    }

    public async UniTask StartGameFlowAsync()
    {
        await Auth.SignInAsync();
        UIDtext.text = StoveManager.Instance.User.Nickname;

        await LoadPlayerDataAsync();
    }

    public void CompleteNicknameRegistration()
    {
        nicknameRegisterTCS?.TrySetResult(true);
    }

    /// <summary>
    /// 플레이어 데이터 로드
    /// </summary>
    public async UniTask LoadPlayerDataAsync()
    {
        await SaveLoad.LoadAsync();
        IsLoaded = true;
    }

    /// <summary>
    /// 리더보드에 플레이어 점수를 업로드합니다.
    /// </summary>
    /// <param name="score">업로드할 점수</param>
    public async UniTask UploadScoreAsync(int score, int queenCharaterID)
    {
        await Leaderboard.UploadScoreAsync(score, queenCharaterID);
    }

    /// <summary>
    /// 리더보드에서 Top 10 플레이어의 점수를 불러옵니다.
    /// </summary>
    public async UniTask LoadLeaderboardTop10Async()
    {
        await Leaderboard.GetTop10ScoresAsync();
    }

    /// <summary>
    /// 현재 플레이어의 리더보드 순위 및 점수를 불러옵니다.
    /// </summary>
    public async UniTask LoadMyRankAsync()
    {
        await Leaderboard.GetMyRankAsync();
    }
}
