using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
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
        if (string.IsNullOrEmpty(str))
        {
            return Vector3.zero;
        }

        str = str.Trim('(', ')');
        string[] xyz = str.Split(',');

        if (xyz.Length != 3)
        {
            return Vector3.zero;
        }

        bool xCheck = float.TryParse(xyz[0], out float x);
        bool yCheck = float.TryParse(xyz[1], out float y);
        bool zCheck = float.TryParse(xyz[2], out float z);

        if (!xCheck || !yCheck || !zCheck)
        {
            return Vector3.zero;
        }

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// string 타입을 Int로 바꿔주는 함수
    /// </summary>
    /// <param name="value"> Int로 바꿀 문자열 </param>
    public static int StringToInt(string value, int defaultValue = 0)
    {
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <summary>
    /// string 타입을 Float로 바꿔주는 함수
    /// </summary>
    /// <param name="value"> Float로 바꿀 문자열 </param>
    public static float StringToFloat(string value, float defaultValue = 0)
    {
        return float.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <summary>
    /// string 타입을 Bool로 바꿔주는 함수
    /// </summary>
    /// <param name="value"> Bool로 바꿀 문자열 </param>
    public static bool StringToBool(string value, bool defaultValue = false)
    {
        return bool.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <summary>
    /// string 타입을 Enum으로 바꿔주는 함수
    /// </summary>
    /// <typeparam name="T"> Enum 타입 </typeparam>
    /// <param name="value"> Enum으로 바꿀 문자열 </param>
    public static T StringToEnum<T>(string value, T defaultValue) where T : struct
    {
        return Enum.TryParse(value, out T result) ? result : defaultValue;
    }

    /// <summary>
    /// string 타입을 int형 배열로 바꿔주는 함수
    /// </summary>
    /// <param name="value"> int형 배열로 바꿀 문자열. 101,102,103 <- 이런 형식이어야 됨 </param>
    /// <returns></returns>
    public static int[] StringToIntArr(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Array.Empty<int>();
        }

        List<int> result = new List<int>();

        foreach (string str in value.Split(","))
        {
            if (int.TryParse(str, out int num))
            {
                result.Add(num);
            }
        }

        return result.ToArray();
    }

    /// <summary>
    /// OverlapBox를 기반으로 Debug.DrawLine 그리기
    /// </summary>
    /// <param name="center"></param>
    /// <param name="size"></param>
    /// <param name="angleDeg"></param>
    /// <param name="color"></param>
    /// <param name="duration"></param>
    public static void DrawBoxCast(Vector2 center, Vector2 size, float angleDeg, Color color, float duration = 0.1f)
    {
        // // 회전값을 라디안으로 변환
        // float angleRad = angleDeg * Mathf.Deg2Rad;
        // Quaternion rotation = Quaternion.Euler(0, 0, angleDeg);

        // // 박스의 4개 꼭짓점 구하기 (기준은 중심에서 오프셋)
        // Vector2 halfSize = size * 0.5f;

        // Vector2 topRight = new Vector2(halfSize.x, halfSize.y);
        // Vector2 topLeft = new Vector2(-halfSize.x, halfSize.y);
        // Vector2 bottomLeft = new Vector2(-halfSize.x, -halfSize.y);
        // Vector2 bottomRight = new Vector2(halfSize.x, -halfSize.y);

        // // 회전 적용 후 월드 좌표로 변환
        // topRight = center + (Vector2)(rotation * topRight);
        // topLeft = center + (Vector2)(rotation * topLeft);
        // bottomLeft = center + (Vector2)(rotation * bottomLeft);
        // bottomRight = center + (Vector2)(rotation * bottomRight);

        // // 박스 테두리 그리기
        // Debug.DrawLine(topRight, topLeft, color, duration);
        // Debug.DrawLine(topLeft, bottomLeft, color, duration);
        // Debug.DrawLine(bottomLeft, bottomRight, color, duration);
        // Debug.DrawLine(bottomRight, topRight, color, duration);
    }

    /// <summary>
    /// OverlapCircle을 기반으로 Debug.DrawLine 그리기
    /// </summary>
    /// <param name="origin"></param>
    /// <param name="radius"></param>
    /// <param name="color"></param>
    /// <param name="duration"></param>
    public static void DrawOverlapCircle(Vector2 origin, float radius, Color color, float duration = 0.1f)
    {
        // // 원을 그리기 위한 360도 각도
        // int segments = 36; // 원을 그릴 때 사용할 점의 수
        // float angleStep = 360f / segments;

        // Vector2 previousPoint = origin + new Vector2(radius, 0); // 처음 점

        // for (int i = 1; i <= segments; i++)
        // {
        //     // 현재 각도를 구하고, 해당 각도로 벡터 계산
        //     float angle = angleStep * i;
        //     Vector2 newPoint = origin + new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad) * radius, Mathf.Sin(angle * Mathf.Deg2Rad) * radius);

        //     // 원을 이루는 점을 그리기
        //     Debug.DrawLine(previousPoint, newPoint, color, duration);

        //     // 이전 점을 현재 점으로 갱신
        //     previousPoint = newPoint;
        // }
    }
}
