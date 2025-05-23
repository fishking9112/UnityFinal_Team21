using JetBrains.Annotations;
using System.Collections.Generic;
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

    public Dictionary<int, MonsterInfo> queenAbilityMonsterStatDic = new Dictionary<int, MonsterInfo>();

    protected override void Awake()
    {
        base.Awake();
        if (monsterData == null)
        {
            monsterData = Addressables.LoadAssetAsync<MonsterData>("MonsterData").WaitForCompletion();
        }
        if (queenAbilityData == null)
        {
            queenAbilityData = Addressables.LoadAssetAsync<QueenAbilityData>("QueenAbilityData").WaitForCompletion();
        }
        if (heroAbilityData == null)
        {
            heroAbilityData = Addressables.LoadAssetAsync<HeroAbilityData>("HeroAbilityData").WaitForCompletion();
        }
        if (heroAbilityLevelUpData == null)
        {
            heroAbilityLevelUpData = Addressables.LoadAssetAsync<HeroAbilityLevelUpData>("HeroAbilityLevelUpData").WaitForCompletion();
        }
        if (queenEnhanceData == null)
        {
            queenEnhanceData = Addressables.LoadAssetAsync<QueenEnhanceData>("QueenEnhanceData").WaitForCompletion();
        }
        if (heroStatusData == null)
        {
            heroStatusData = Addressables.LoadAssetAsync<HeroStatusData>("HeroStatusData").WaitForCompletion();
        }
        if (queenActiveSkillData == null)
        {
            queenActiveSkillData = Addressables.LoadAssetAsync<QueenActiveSkillData>("QueenActiveSkillData").WaitForCompletion();
        }
        if (buffData == null)
        {
            buffData = Addressables.LoadAssetAsync<BuffData>("BuffData").WaitForCompletion();
        }
        if (trophyData == null)
        {
            trophyData = Addressables.LoadAssetAsync<TrophyData>("TrophyData").WaitForCompletion();
        }
        if (eventData == null)
        {
            eventData = Addressables.LoadAssetAsync<EventData>("EventData").WaitForCompletion();
        }
        if (queenStatusData == null)
        {
            queenStatusData = Addressables.LoadAssetAsync<QueenStatusData>("QueenStatusData").WaitForCompletion();
        }
        if (queenPassiveSkillData == null)
        {
            queenPassiveSkillData = Addressables.LoadAssetAsync<QueenPassiveSkillData>("QueenPassiveSkillData").WaitForCompletion();
        }
        if (toolTipData == null)
        {
            toolTipData = Addressables.LoadAssetAsync<ToolTipData>("ToolTipData").WaitForCompletion();
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
