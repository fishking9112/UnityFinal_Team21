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

    // 모든 데이터 딕셔너리
    public Dictionary<int, MonsterInfo> monsterDic = new Dictionary<int, MonsterInfo>();
    public Dictionary<int, QueenAbilityInfo> queenAbilityDic = new Dictionary<int, QueenAbilityInfo>();
    public Dictionary<int, HeroAbilityInfo> heroAbilityDic = new Dictionary<int, HeroAbilityInfo>();

    protected override void Awake()
    {
        base.Awake();

        Init<MonsterInfo>(monsterData.infoList,monsterDic);
        Init<QueenAbilityInfo>(queenAbilityData.infoList, queenAbilityDic);
        Init<HeroAbilityInfo>(heroAbilityData.infoList, heroAbilityDic);
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
