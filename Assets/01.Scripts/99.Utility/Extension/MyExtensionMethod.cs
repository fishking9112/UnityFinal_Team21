using System;
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
        return gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();
    }

    /// <summary>
    /// 리스트 깊은 복사 (List가 int같은 값 타입 일 경우)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <returns></returns>
    public static List<T> Clone<T>(this List<T> list)
    {
        var retList = new List<T>();
        foreach (var item in list)
        {
            retList.Add(item);
        }
        return retList;
    }

    /// <summary>
    /// 리스트 깊은 복사 (List가 Class같은 참조 타입 일 경우)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    /// <param name="cloneFunc"></param>
    /// <returns></returns>
    public static List<T> Clone<T>(this List<T> list, Func<T, T> cloneFunc)
    {
        var retList = new List<T>(list.Count);
        foreach (var item in list)
        {
            retList.Add(cloneFunc(item));
        }
        return retList;
    }
}