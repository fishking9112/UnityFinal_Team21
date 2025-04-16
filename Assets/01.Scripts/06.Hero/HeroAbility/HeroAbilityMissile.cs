using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class HeroAbilityMissile : HeroAbilitySystem
{
    private Vector2 fireDir = Vector2.left;

    private int count;

    private Hero hero;

    private ObjectPoolManager objectPoolManager;

    /// <summary>
    /// 선언과 동시에 호출하기. 값 입력
    /// </summary>
    protected override void Start()
    {
        heroAbilityInfo = DataManager.Instance.heroAbilityDic[102];

        base.Start();

        hero = GameManager.Instance.hero;
        objectPoolManager = ObjectPoolManager.Instance;

        count = heroAbilityInfo.count_Base;

        AddAbility();
    }

    /// <summary>
    /// 어떤 능력인지 구현하는 곳
    /// 오브젝트풀로 구현해야함
    /// </summary>
    protected override void ActionAbility()
    {
        target = hero.FindNearestTarget();
        ShootBullet().Forget();
    }

    private async UniTaskVoid ShootBullet()
    {
        float angle;

        if (target == null)
        {
            angle = 0;
        }
        else
        {
            angle = Mathf.Atan2(target.transform.position.y - hero.transform.position.y,
                target.transform.position.x - hero.transform.position.x) * Mathf.Rad2Deg;
        }

        for (int i = 0; i < count; i++)
        {
            var bullet = objectPoolManager.GetObject<HeroBullet>("bullet", hero.transform.position);
            bullet.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
        }
    }

    /// <summary>
    /// 임시로 만든 함수
    /// </summary>
    /// <param name="cnt"></param>
    public void IncreaseBulletCount(int cnt)
    {
        count += cnt;
    }

    public override void AbilityLevelUp()
    {
        base.AbilityLevelUp();

        // Missile이 레벨업 시 증가해야 되는 스텟 증가 추가
    }

    public override void DespawnAbility()
    {

    }
}
