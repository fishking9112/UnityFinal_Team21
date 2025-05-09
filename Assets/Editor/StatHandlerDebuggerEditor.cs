
#if UNITY_EDITOR // 이 코드는 Unity 에디터 환경에서만 컴파일됨

using UnityEditor; // Unity 에디터 관련 API
using UnityEngine; // Unity 엔진의 기본 클래스
using System.Collections.Generic; // 제네릭 컬렉션 사용
using System.Collections;
using System.Reflection; // 비제네릭 컬렉션 사용

[CustomEditor(typeof(StatHandler), true)]
public class StatHandlerDebuggerEditor : Editor // Unity의 Editor 클래스를 상속
{
    private bool showRxDebug = true; // Rx 디버깅 뷰의 접기/펼치기 상태를 저장

    public override void OnInspectorGUI() // 인스펙터 UI를 그리는 메서드 오버라이드
    {
        base.OnInspectorGUI(); // 기본 인스펙터 그리기

        // if (!Application.isPlaying) return; // 플레이 중일 때만 표시 (현재 비활성화)

        DrawModelDebugView();
    }

    private void DrawModelDebugView()
    {
        EditorGUILayout.Space(); // 여백 추가

        showRxDebug = EditorGUILayout.Foldout(showRxDebug, "디버그 뷰", true);
        if (!showRxDebug) return; // 접혀 있으면 그리지 않음

        EditorGUI.indentLevel++; // 들여쓰기 레벨 증가

        StatHandler handler = (StatHandler)target;

        // 리플렉션
        var fields = typeof(StatHandler).GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var field in fields)
        {
            if (field.FieldType == typeof(ModFloat))
            {
                ModFloat mod = field.GetValue(handler) as ModFloat;
                if (mod != null)
                {
                    string name = ObjectNames.NicifyVariableName(field.Name); // 보기 좋게 포맷
                    EditorGUILayout.LabelField(name, mod.BuildDebugFormula());
                }
            }
        }

        EditorGUI.indentLevel--; // 들여쓰기 복원
    }
}

#endif // UNITY_EDITOR 조건부 컴파일 끝