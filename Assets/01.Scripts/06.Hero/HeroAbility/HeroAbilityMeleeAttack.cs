using Cysharp.Threading.Tasks;
using UnityEngine;

public class HeroAbilityMeleeAttack : HeroAbilitySystem
{
    private GameObject sword;
    private Hero hero;

    private Animator animator;

    private HeroBasicSword basicSword;

    protected override void Start()
    {
        hero = transform.GetComponent<Hero>();

        sword = transform.Find("Sword").gameObject;
        basicSword = GetComponentInChildren<HeroBasicSword>();
        animator = sword.GetComponent<Animator>();
        basicSword.Init(damage, knockback);
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
        
        await UniTask.WaitUntil(()=> animator.GetCurrentAnimatorStateInfo(0).normalizedTime>=1f);
        sword.SetActive(false);

    }

    public override void AbilityLevelUp()
    {
        base.AbilityLevelUp();

        // MeleeAttack이 레벨업 시 증가해야 되는 스텟 증가 추가
    }

    public override void DespawnAbility()
    {
       
    }
}