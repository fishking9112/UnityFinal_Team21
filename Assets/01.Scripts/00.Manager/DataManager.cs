using System.Collections.Generic;
using UnityEngine;

// Info들의 ID가 무조건 존재해야 하기 때문에 인터페이스를 사용하여 무조건 구현 하도록 함
public interface IInfo
{
    int ID { get; }
}

public class DataManager : MonoSingleton<DataManager>
{
    [Header("Data SO")]
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private QueenAbilityData queenAbilityData;
    [SerializeField] private HeroAbilityData heroAbilityData;
    [SerializeField] private QueenEnhanceData queenEnhanceData;
    [SerializeField] private HeroStatusData heroStatusData;
    [SerializeField] private QueenActiveSkillData queenActiveSkillData;
    [SerializeField] private BuffData buffData;
    [SerializeField] private TrophyData trophyData;

    // iconData는 id값으로 초기화 하지 않으므로, iconData 안에 Dictionary 존재
    public IconData iconData;

    // 모든 데이터 딕셔너리
    public Dictionary<int, MonsterInfo> monsterDic = new Dictionary<int, MonsterInfo>();
    public Dictionary<int, QueenAbilityInfo> queenAbilityDic = new Dictionary<int, QueenAbilityInfo>();
    public Dictionary<int, HeroAbilityInfo> heroAbilityDic = new Dictionary<int, HeroAbilityInfo>();
    public Dictionary<int, QueenEnhanceInfo> queenEnhanceDic = new Dictionary<int, QueenEnhanceInfo>();
    public Dictionary<int, HeroStatusInfo> heroStatusDic = new Dictionary<int, HeroStatusInfo>();
    public Dictionary<int, QueenActiveSkillInfo> queenActiveSkillDic = new Dictionary<int, QueenActiveSkillInfo>();
    public Dictionary<int, BuffInfo> buffDic = new Dictionary<int, BuffInfo>();
    public Dictionary<int, TrophyInfo> trophyDic = new Dictionary<int, TrophyInfo>();

    protected override void Awake()
    {
        base.Awake();

        Init<MonsterInfo>(monsterData.infoList, monsterDic);
        Init<QueenAbilityInfo>(queenAbilityData.infoList, queenAbilityDic);
        Init<HeroAbilityInfo>(heroAbilityData.infoList, heroAbilityDic);
        Init<QueenEnhanceInfo>(queenEnhanceData.infoList, queenEnhanceDic);
        Init<HeroStatusInfo>(heroStatusData.infoList, heroStatusDic);
        Init<QueenActiveSkillInfo>(queenActiveSkillData.infoList, queenActiveSkillDic);
        Init<BuffInfo>(buffData.infoList, buffDic);
        Init<TrophyInfo>(trophyData.infoList, trophyDic);
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
