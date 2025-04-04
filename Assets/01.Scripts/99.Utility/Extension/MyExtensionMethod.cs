using System.Collections.Generic;
using UnityEngine;

public static class MyExtensionMethod
{
    // 컴포넌트 있으면 Get 없으면 Add 후 반환
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        var component = gameObject.GetComponent<T>();
        if (component == null) gameObject.AddComponent<T>();
        return component;
    }
}