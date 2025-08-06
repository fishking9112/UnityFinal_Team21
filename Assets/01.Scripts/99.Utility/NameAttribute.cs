using UnityEngine;

/// <summary>
/// Inspector에 표시될 변수의 이름을 바꿔주는 어트리뷰트입니다.
/// </summary>
public class NameAttribute : PropertyAttribute
{
    public string name;

    public NameAttribute(string name)
    {
        this.name = name;
    }
}
