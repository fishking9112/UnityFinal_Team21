using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum ModifierType
{
    Plus,
    Multiply,
}

public sealed class ModFloat
{
    private float origin; // 초기 원본 값
    private float cachedValue; // 계산된 값 캐싱

    public float Value => cachedValue; // 최종 값 get

    // 스킬의 id, 증가량 float
    private readonly Dictionary<int, float> plus = new();
    private readonly Dictionary<int, float> multiply = new();

    // 계산에 사용되는 중간 변수들
    private float sum, mul;

    // 실제 계산 구현
    private void CalculateValue()
    {
        sum = 0f;
        foreach (var v in plus.Values) sum += v;

        mul = 1f;
        foreach (var v in multiply.Values) mul *= v;

        float result = (origin + sum) * mul;
        cachedValue = result;
    }

    // 모든 수정자 제거 및 재계산
    public void ClearAll()
    {
        plus.Clear();
        multiply.Clear();

        CalculateValue();
    }

    public void SetOrigin(float value)
    {
        origin = value;
        CalculateValue(); // 즉시 재계산
    }

    public void AddOrigin(float value)
    {
        origin += value;
        CalculateValue(); // 즉시 재계산
    }

    public void AddModifier(ModifierType type, int key, float value)
    {
        switch (type)
        {
            case ModifierType.Plus: plus[key] = value; break;
            case ModifierType.Multiply: multiply[key] = value; break;
        }
        CalculateValue(); // 즉시 재계산
    }
    public void RemoveModifier(ModifierType type, int key)
    {
        bool removed = type switch
        {
            ModifierType.Plus => plus.Remove(key),
            ModifierType.Multiply => multiply.Remove(key),
            _ => false
        };
        if (removed) CalculateValue(); // 변경 있을 때만 재계산
    }

    public string BuildDebugFormula()
    {
        var sb = new StringBuilder();
        sb.Append('(').Append(origin).Append('+').Append(sum).Append(')');
        sb.Append(" * ").Append(mul);

        return sb.ToString();
    }

}
