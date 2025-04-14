using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class Utils
{
    // 천단위 콤마
    public static string GetThousandCommaText(int data)
    {
        if (data == 0) return "0";
        return string.Format("{0:#,###}", data);
    }

    // 시간 분:초
    // 1시간 이상은 상정하지 않았음
    public static string GetMMSSTime(int time)
    {
        TimeSpan ts = TimeSpan.FromSeconds(time);
        var str = string.Format("{0:D2} : {1:D2}", ts.Minutes, ts.Seconds);

        return str;
    }

    // 정 디버그.로그를 쓰고싶다면 이걸로
    public static void Log(string message)
    {
#if UNITY_EDITOR
        Debug.Log(message);
#endif
    }

    // 정 디버그.로그를 쓰고싶다면 이걸로
    public static void LogError(string message)
    {
#if UNITY_EDITOR
        Debug.LogError(message);
#endif
    }

    // 정 디버그.로그를 쓰고싶다면 이걸로
    public static void LogWarning(string message)
    {
#if UNITY_EDITOR
        Debug.LogWarning(message);
#endif
    }

    /// <summary>
    /// delayedTime초 후에 action 실행. 호출부에 using Unitask 필요 없음
    /// </summary>
    /// <param name="action"></param>
    /// <param name="delayTime"></param>
    public static void DelayedTimeAction(Action action, float delayTime)
    {
        DelayedAction(action, delayTime).Forget();
    }
    private static async UniTaskVoid DelayedAction(Action action, float delayTime)
    {
        await UniTask.Delay(TimeSpan.FromSeconds(delayTime));
        action?.Invoke();
    }

    /// <summary>
    /// string타입을 Vector3로 바꿔주는 함수
    /// </summary>
    /// <param name="str"> Vector3로 바꿀 문자열. (x, y, z) <- 이런 형식이어야 됨 </param>
    public static Vector3 StringToVector3(string str)
    {
        str = str.Trim('(', ')');

        string[] xyz = str.Split(',');

        float x = float.Parse(xyz[0]);
        float y = float.Parse(xyz[1]);
        float z = float.Parse(xyz[2]);

        return new Vector3(x, y, z);
    }
}
