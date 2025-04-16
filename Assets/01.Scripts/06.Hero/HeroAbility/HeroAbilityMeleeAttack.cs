using Cysharp.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class HeroAbilityMeleeAttack : HeroAbilitySystem
{
    private GameObject sword;
    private Hero hero;

    private Animator animator;



    protected override void Start()
    {
        heroAbilityInfo = DataManager.Instance.heroAbilityDic[101];
        base.Start();


        hero = transform.GetComponent<Hero>();

        sword = transform.Find("Sword").gameObject;
        animator = sword.GetComponent<Animator>();
        sword.SetActive(false);

        AddAbility();
    }

    protected override void ActionAbility()
    {
        target = hero.FindNearestTarget();

        SwingSword().Forget();
    }


    private async UniTaskVoid SwingSword()
    {
        float angle;

        if (target==null)
        {
            angle = 0;
        }
        else
        {
            angle = Mathf.Atan2(target.transform.position.y - hero.transform.position.y,
                target.transform.position.x - hero.transform.position.x) * Mathf.Rad2Deg;
        }

        sword.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        sword.SetActive(true);
        
        await UniTask.WaitUntil(()=> animator.GetCurrentAnimatorStateInfo(0).normalizedTime>=0.5f);

        // 충돌처리
        OverlapCheck(angle);

        await UniTask.WaitUntil(()=> animator.GetCurrentAnimatorStateInfo(0).normalizedTime>=1f);
        sword.SetActive(false);

    }

    public override void AbilityLevelUp()
    {
        base.AbilityLevelUp();

        // MeleeAttack이 레벨업 시 증가해야 되는 스텟 증가 추가
    }

    private void OverlapCheck(float angle)
    {
        float angleRad = angle * Mathf.Deg2Rad;
        Vector2 local= (Vector2)transform.position+ new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad));

        Collider2D[] col = Physics2D.OverlapCircleAll(local, size.x,1<<7);
        Utils.DrawOverlapCircle(local, 1, Color.red);
        foreach(var c in col)
        {
            if (MonsterManager.Instance.monsters.TryGetValue(c.gameObject, out var monster))
            {
                monster.TakeDamaged(damage);
                Utils.Log("맞음");
            }
        }
    }

    public override void DespawnAbility()
    {
       
    }
}