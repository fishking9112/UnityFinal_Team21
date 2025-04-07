using System.Collections.Generic;
using UnityEngine;

public static class MyExtensionMethod
{
    /// <summary>
    /// 컴포넌트 있으면 Get 없으면 Add 후 반환
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="gameObject">반환 될 게임 오브젝트</param>
    /// <returns></returns>
    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        var component=gameObject.GetComponent<T>();
        if(component == null) gameObject.AddComponent<T>();
        return component;
    }
}