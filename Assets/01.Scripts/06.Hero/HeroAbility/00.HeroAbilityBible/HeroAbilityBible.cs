using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HeroAbilityBible : HeroAbilitySystem
{
    private ObjectPoolManager objectPoolManager;
    private List<IPoolable> bibleList = new List<IPoolable>();

    private bool maxUpgrade;

    public override void Initialize(int id)
    {
        base.Initialize(id);


    }

    private void Start()
    {
        objectPoolManager = ObjectPoolManager.Instance;

        maxUpgrade = false;
        AddAbility();

        //// 테스트 레벨업
        //AbilityLevelUp();
        //AbilityLevelUp();
        //AbilityLevelUp();
    }

    protected override void ActionAbility()
    {
        SummonBible();
    }

    // 성경책 생성
    private void SummonBible()
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

            if(curLevel == maxLevel && !maxUpgrade)
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
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        bible?.OnDespawn();
        bibleList.Remove(bible);
    }

    public override void AbilityLevelUp()
    {
        base.AbilityLevelUp();

    }

    public override void DespawnAbility()
    {
        foreach(var bible in bibleList)
        {
            bible?.OnDespawn();
        }

        bibleList.Clear();
    }
    public override void SetAbilityLevel(int level)
    {
        base.SetAbilityLevel(level);
    }
}
