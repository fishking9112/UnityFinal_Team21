using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroAbilityBible : HeroAbilitySystem
{
    private ObjectPoolManager objectPoolManager;
    private List<IPoolable> bibleList = new List<IPoolable>();

    [Header("Bible Stat")]
    [SerializeField] private float speed;
    [SerializeField] private float duration;
    [SerializeField] private int count;

    private bool maxUpgrade;

    protected override void Start()
    {
        heroAbilityInfo = DataManager.Instance.heroAbilityDic[103];

        base.Start();

        objectPoolManager = ObjectPoolManager.Instance;

        speed = heroAbilityInfo.speed_Base;
        duration = heroAbilityInfo.duration_Base;
        count = heroAbilityInfo.count_Base;

        maxUpgrade = false;

        AddAbility();

        // 테스트 레벨업
        AbilityLevelUp();
        AbilityLevelUp();
        AbilityLevelUp();
    }

    protected override void ActionAbility()
    {
        SummonBible().Forget();
    }

    // 성경책 생성
    private async UniTaskVoid SummonBible()
    {
        // 만렙일 경우 무한 지속
        if (maxUpgrade)
        {
            return;
        }

        float angle = 360f / count;

        for (int i = 0; i < count; i++)
        {
            // 플레이어를 기준으로 pivot만큼 떨어진 곳에 생성
            float summonAngle = i * angle * Mathf.Deg2Rad;
            Vector3 summonPosition = transform.position + new Vector3(MathF.Cos(summonAngle), MathF.Sin(summonAngle), 0f) + pivot;

            Bible bible = objectPoolManager.GetObject<Bible>("Bible", summonPosition);

            // 소환한 성경책 초기화
            //Bible bible = biblePrefab.GetComponent<Bible>();
            bible.target = this.transform;
            bible.radius = pivot.magnitude;
            bible.speed = speed;
            bible.angle = summonAngle;

            bibleList.Add(bible);

            if(curLevel == maxLevel)
            {
                maxUpgrade = true;
            }
            else
            {
                // 만렙이 아닐 경우 지속시간이 지나면 디스폰
                DespawnBible(bible, duration).Forget();
            }
        }
    }

    private async UniTaskVoid DespawnBible(IPoolable bible, float delay)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(duration));
        bible?.OnDespawn();
        bibleList.Remove(bible);
    }

    public override void AbilityLevelUp()
    {
        base.AbilityLevelUp();

        speed += heroAbilityInfo.speed_LevelUp;
        duration += heroAbilityInfo.duration_LevelUp;
        count += heroAbilityInfo.count_LevelUp;
    }

    public override void DespawnAbility()
    {
        foreach(var bible in bibleList)
        {
            bible?.OnDespawn();
        }

        bibleList.Clear();
    }
}
