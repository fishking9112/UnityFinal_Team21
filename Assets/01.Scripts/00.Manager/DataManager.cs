using System.Collections.Generic;
using UnityEngine;

public interface IInfo
{
    int ID { get; }
}

public class DataManager : MonoSingleton<DataManager>
{
    [SerializeField] private MonsterData monsterData;
    [SerializeField] private QueenAbilityData queenAbilityData;

    public Dictionary<int, MonsterInfo> monsterDic = new Dictionary<int, MonsterInfo>();
    public Dictionary<int, QueenAbilityInfo> queenAilityDic = new Dictionary<int, QueenAbilityInfo>();

    protected override void Awake()
    {
        Init<MonsterInfo>(monsterData.infoList,monsterDic);
        Init<QueenAbilityInfo>(queenAbilityData.infoList, queenAilityDic);

        print(monsterDic[0].name);
        print(queenAilityDic[0].name);
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
