using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBasicSword : MonoBehaviour
{

    private LayerMask targetLayer;

    private float damage;
    private float knockback;


    private void Start()
    {
        targetLayer = 7;//1 << LayerMask.GetMask("Monster");

    }

    public void Init(float dmg, float kback)
    {
        SetDamage(dmg);
        SetKnockback(kback);
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }
    public void SetKnockback(float knockback)
    {
        this.knockback = knockback;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer==targetLayer)
        {
            Utils.Log("칼에 맞음");
            // 데미지 주기
            // 넉백 시키기
        }
    }

}
