using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    public float healthPositionHeight; // 체력 위치 높이조정
    public HealthUI healthUI;

    float maxPoint;
    float currentPoint;

    public void Init(float hp)
    {
        maxPoint = hp;
        currentPoint = hp;
    }

    public void SetMax(float maxHP)
    {
        // 체력이 더 높아지면 현재 체력(+)
        if (this.maxPoint < maxHP) currentPoint += maxHP - this.maxPoint;
        this.maxPoint = maxHP;
    }

    public void Damage(float damage)
    {
        currentPoint -= damage;
        if (currentPoint <= 0f) currentPoint = 0f;
    }

    public void Heal(float heal)
    {
        currentPoint += heal;
        if (currentPoint >= maxPoint) currentPoint = maxPoint;
    }
}
