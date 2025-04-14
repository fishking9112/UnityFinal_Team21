using System.Collections.Generic;
using UnityEngine;

public interface IInfo
{
    int ID { get; }
}

public class DataManager : MonoSingleton<DataManager>
{
    [Header("Data SO")]
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private QueenAbilityData queenAbilityData;

    // 모든 데이터 딕셔너리
    public Dictionary<int, MonsterInfo> monsterDic = new Dictionary<int, MonsterInfo>();
    public Dictionary<int, QueenAbilityInfo> queenAilityDic = new Dictionary<int, QueenAbilityInfo>();

    protected override void Awake()
    {
        base.Awake();

        Init<MonsterInfo>(monsterData.infoList,monsterDic);
        Init<QueenAbilityInfo>(queenAbilityData.infoList, queenAilityDic);
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
