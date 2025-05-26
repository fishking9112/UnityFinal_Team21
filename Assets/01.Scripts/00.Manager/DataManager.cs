using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;

// Info들의 ID가 무조건 존재해야 하기 때문에 인터페이스를 사용하여 무조건 구현 하도록 함
public interface IInfo
{
    int ID { get; }
    string Name { get; }
    string Description { get; }
    string Icon { get; }
}

public class DataManager : MonoSingleton<DataManager>
{
    [Header("Data SO")]
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private QueenAbilityData queenAbilityData;
    [SerializeField] private HeroAbilityData heroAbilityData;
    [SerializeField] private HeroAbilityLevelUpData heroAbilityLevelUpData;
    [SerializeField] private QueenEnhanceData queenEnhanceData;
    [SerializeField] private HeroStatusData heroStatusData;
    [SerializeField] private QueenActiveSkillData queenActiveSkillData;
    [SerializeField] private BuffData buffData;
    [SerializeField] private TrophyData trophyData;
    [SerializeField] private EventData eventData;
    [SerializeField] private QueenStatusData queenStatusData;
    [SerializeField] private QueenPassiveSkillData queenPassiveSkillData;
    [SerializeField] private ToolTipData toolTipData;
    [SerializeField] private UIToolTipData uiToolTipData;

    // iconData는 id값으로 초기화 하지 않으므로, iconData 안에 Dictionary 존재
    public SpriteAtlas iconAtlas;
    public SpriteAtlas tooltipAtlas;

    // 모든 데이터 딕셔너리
    public Dictionary<int, MonsterInfo> monsterDic = new Dictionary<int, MonsterInfo>();
    public Dictionary<int, QueenAbilityInfo> queenAbilityDic = new Dictionary<int, QueenAbilityInfo>();
    public Dictionary<int, HeroAbilityInfo> heroAbilityDic = new Dictionary<int, HeroAbilityInfo>();
    public Dictionary<int, HeroAbilityLevelUpInfo> heroAbilityLevelUpDic = new Dictionary<int, HeroAbilityLevelUpInfo>();
    public Dictionary<int, QueenEnhanceInfo> queenEnhanceDic = new Dictionary<int, QueenEnhanceInfo>();
    public Dictionary<int, HeroStatusInfo> heroStatusDic = new Dictionary<int, HeroStatusInfo>();
    public Dictionary<int, QueenActiveSkillInfo> queenActiveSkillDic = new Dictionary<int, QueenActiveSkillInfo>();
    public Dictionary<int, BuffInfo> buffDic = new Dictionary<int, BuffInfo>();
    public Dictionary<int, TrophyInfo> trophyDic = new Dictionary<int, TrophyInfo>();
    public Dictionary<int, EventInfo> eventDic = new Dictionary<int, EventInfo>();
    public Dictionary<int, QueenStatusInfo> queenStatusDic = new Dictionary<int, QueenStatusInfo>();
    public Dictionary<int, QueenPassiveSkillInfo> queenPassiveSkillDic = new Dictionary<int, QueenPassiveSkillInfo>();
    public Dictionary<int, ToolTipInfo> toolTipDic = new Dictionary<int, ToolTipInfo>();
    public Dictionary<int, UIToolTipInfo> uiToolTipDic = new Dictionary<int, UIToolTipInfo>();

    public Dictionary<int, MonsterInfo> queenAbilityMonsterStatDic = new Dictionary<int, MonsterInfo>();

    protected override void Awake()
    {
        base.Awake();
        InitAllData().Forget();
    }

    private async UniTaskVoid InitAllData()
    {
        if (monsterData == null)
        {
            monsterData = await Addressables.LoadAssetAsync<MonsterData>("MonsterData");
        }
        if (queenAbilityData == null)
        {
            queenAbilityData = await Addressables.LoadAssetAsync<QueenAbilityData>("QueenAbilityData");
        }
        if (heroAbilityData == null)
        {
            heroAbilityData = await Addressables.LoadAssetAsync<HeroAbilityData>("HeroAbilityData");
        }
        if (heroAbilityLevelUpData == null)
        {
            heroAbilityLevelUpData = await Addressables.LoadAssetAsync<HeroAbilityLevelUpData>("HeroAbilityLevelUpData");
        }
        if (queenEnhanceData == null)
        {
            queenEnhanceData = await Addressables.LoadAssetAsync<QueenEnhanceData>("QueenEnhanceData");
        }
        if (heroStatusData == null)
        {
            heroStatusData = await Addressables.LoadAssetAsync<HeroStatusData>("HeroStatusData");
        }
        if (queenActiveSkillData == null)
        {
            queenActiveSkillData = await Addressables.LoadAssetAsync<QueenActiveSkillData>("QueenActiveSkillData");
        }
        if (buffData == null)
        {
            buffData = await Addressables.LoadAssetAsync<BuffData>("BuffData");
        }
        if (trophyData == null)
        {
            trophyData = await Addressables.LoadAssetAsync<TrophyData>("TrophyData");
        }
        if (eventData == null)
        {
            eventData = await Addressables.LoadAssetAsync<EventData>("EventData");
        }
        if (queenStatusData == null)
        {
            queenStatusData = await Addressables.LoadAssetAsync<QueenStatusData>("QueenStatusData");
        }
        if (queenPassiveSkillData == null)
        {
            queenPassiveSkillData = await Addressables.LoadAssetAsync<QueenPassiveSkillData>("QueenPassiveSkillData");
        }
        if (toolTipData == null)
        {
            toolTipData = await Addressables.LoadAssetAsync<ToolTipData>("ToolTipData");
        }
        if (uiToolTipData == null)
        {
            toolTipData = Addressables.LoadAssetAsync<ToolTipData>("UIToolTipData").WaitForCompletion();
        }

        Init<MonsterInfo>(monsterData.infoList, monsterDic);
        Init<QueenAbilityInfo>(queenAbilityData.infoList, queenAbilityDic);
        Init<HeroAbilityInfo>(heroAbilityData.infoList, heroAbilityDic);
        Init<HeroAbilityLevelUpInfo>(heroAbilityLevelUpData.infoList, heroAbilityLevelUpDic);
        Init<QueenEnhanceInfo>(queenEnhanceData.infoList, queenEnhanceDic);
        Init<HeroStatusInfo>(heroStatusData.infoList, heroStatusDic);
        Init<QueenActiveSkillInfo>(queenActiveSkillData.infoList, queenActiveSkillDic);
        Init<BuffInfo>(buffData.infoList, buffDic);
        Init<TrophyInfo>(trophyData.infoList, trophyDic);
        Init<EventInfo>(eventData.infoList, eventDic);
        Init<QueenStatusInfo>(queenStatusData.infoList, queenStatusDic);
        Init<QueenPassiveSkillInfo>(queenPassiveSkillData.infoList, queenPassiveSkillDic);
        Init<ToolTipInfo>(toolTipData.infoList, toolTipDic);
        Init<UIToolTipInfo>(uiToolTipData.infoList, uiToolTipDic);

        foreach (var monsterData in monsterDic.Values)
        {
            queenAbilityMonsterStatDic[monsterData.ID] = new MonsterInfo(monsterData);
        }
    }

    // 딕셔너리 초기화
    private void Init<T>(List<T> list, Dictionary<int, T> dic) where T : IInfo
    {
        dic.Clear();

        foreach (var item in list)
        {
            if (!dic.ContainsKey(item.ID))
            {
                dic.Add(item.ID, item);
            }
        }
    }
}
