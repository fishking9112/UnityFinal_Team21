using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StatHandler : MonoBehaviour
{
    public ModFloat health = new();
    public ModFloat moveSpeed = new();
    public ModFloat attack = new();
    public ModFloat attackRange = new();
    public ModFloat attackSpeed = new();

    public void Init(float health, float moveSpeed, float attack = 0f, float attackRange = 0f, float attackSpeed = 0f)
    {
        ClearAll();
        this.health.SetOrigin(health);
        this.moveSpeed.SetOrigin(moveSpeed);
        this.attack.SetOrigin(attack);
        this.attackRange.SetOrigin(attackRange);
        this.attackSpeed.SetOrigin(attackSpeed);
    }

    private void ClearAll()
    {
        this.health.ClearAll();
        this.moveSpeed.ClearAll();
        this.attack.ClearAll();
        this.attackRange.ClearAll();
        this.attackSpeed.ClearAll();
    }
}
