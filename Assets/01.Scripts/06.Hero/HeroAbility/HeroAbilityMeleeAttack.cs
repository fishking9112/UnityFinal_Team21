using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HeroAbilityMeleeAttack : HeroAbilitySystem
{
    private GameObject sword;
    private Hero hero;

    private Animator animator;

    private float damage;
    private float knockback;


    private HeroBasicSword basicSword;

    // Start is called before the first frame update
    void Start()
    {
        hero = transform.GetComponent<Hero>();

        sword = transform.Find("Sword").gameObject;
        basicSword = GetComponentInChildren<HeroBasicSword>();
        animator = sword.GetComponent<Animator>();
        delayTime = 3;
        damage = 5;
        knockback = 1;
        basicSword.Init(damage, knockback);

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

    public override void AbilityLevelUp(int nowLv)
    {
        switch (nowLv)
        {
            case 0:
            case 1:
            case 4:
                damage += 1;
                basicSword.SetDamage(damage);
                break;
            case 2:
            case 3:
                knockback += 1;
                basicSword.SetKnockback(knockback);
                break;
        }

    }


}