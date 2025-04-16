using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class HeroAbilityMissile : HeroAbilitySystem
{
    private Vector2 fireDir = Vector2.left;


    private float count;
    private float pierce;
    private float speed;
    private Hero hero;

    private ObjectPoolManager objectPoolManager;

    /// <summary>
    /// 선언과 동시에 호출하기. 값 입력
    /// </summary>
    protected override void Start()
    {
        heroAbilityInfo = DataManager.Instance.heroAbilityDic[102];

        base.Start();

        hero = this.GetComponent<Hero>();
        objectPoolManager = ObjectPoolManager.Instance;

        count = heroAbilityInfo.count_Base;
        pierce = heroAbilityInfo.piercing_Base;
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
            var bullet = objectPoolManager.GetObject<HeroBullet>("Bullet", hero.transform.position);
            bullet.SetBullet(heroAbilityInfo.duration_Base, pierce, damage, speed);
            bullet.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
        }
    }

    public override void AbilityLevelUp()
    {
        base.AbilityLevelUp();

        // Missile이 레벨업 시 증가해야 되는 스텟 증가 추가
        count+= heroAbilityInfo.count_LevelUp;
        pierce += heroAbilityInfo.piercing_LevelUp;
        speed += heroAbilityInfo.speed_LevelUp;
    }

    public override void DespawnAbility()
    {

    }
}
