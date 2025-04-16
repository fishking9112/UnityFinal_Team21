using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{
    [Header("보스만 체력바 설정")]
    [SerializeField] private HealthUI healthUI;

    float maxPoint;
    ReactiveProperty<float> currentPoint = new();

    void Start()
    {
        currentPoint.AddAction(_ => RefrashUI());
    }

    public void Init(float maxPoint)
    {
        this.maxPoint = maxPoint;
        currentPoint.Value = maxPoint;
    }

    public void SetMaxPoint(float maxPoint)
    {
        // 만약 체력이 더 높아지면 그만큼 +
        float diff = this.maxPoint < maxPoint ? maxPoint - this.maxPoint : 0;
        this.maxPoint = maxPoint;
        // 현재 maxPoint 보다 크면 maxPoint로 줄어들고 아니면 그대로에서 +diff
        currentPoint.Value = (currentPoint.Value + diff <= maxPoint) ? currentPoint.Value + diff : maxPoint;
    }

    public void Damage(float damage)
    {
        if (currentPoint.Value - damage <= 0f) currentPoint.Value = 0f;
        else currentPoint.Value -= damage;
    }

    public bool IsDie()
    {
        if (currentPoint.Value <= 0f) return true;
        else return false;
    }

    public void Heal(float heal)
    {
        currentPoint.Value += heal;
        if (currentPoint.Value >= maxPoint) currentPoint.Value = maxPoint;
    }

    public void RefrashUI()
    {
        healthUI?.SetAmount(currentPoint.Value / maxPoint);
    }
}
