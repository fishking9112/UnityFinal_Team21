#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MyKebabMenu
{
    /// <summary>
    /// 버튼에 케밥메뉴 생성에 대한 간단한 예시
    /// </summary>
    /// <param name="command"></param>
    [MenuItem("CONTEXT/Button/⚡ 버튼 커스텀 액션")]
    private static void CustomButtonAction(MenuCommand command)
    {
        Button button = (Button)command.context;
        Debug.Log($"[{button.name}] 버튼에서 커스텀 액션 실행됨!");
    }

    /// <summary>
    /// CanvasScaler 1920 x 1080
    /// </summary>
    /// <param name="command"></param>
    [MenuItem("CONTEXT/CanvasScaler/1920x1080")]
    private static void CanvasScaler1920x1080(MenuCommand command)
    {
        // 1920 x 1080 크기와 Expand를 자동으로 설정
        CanvasScaler canvasScaler = (CanvasScaler)command.context;
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
    }

    /// <summary>
    /// Text를 TextToTMPro로 간편하게 수정
    /// </summary>
    /// <param name="command"></param>
    [MenuItem("CONTEXT/Text/TextToTMPro")]
    private static void TextToTMPro(MenuCommand command)
    {
        // 기존 Text 컴포넌트 가져오기
        Text oldText = (Text)command.context;
        GameObject obj = oldText.gameObject;

        // 기존 Text 제거 (TMPro랑 중복해서 생성할 수 없음)
        Undo.DestroyObjectImmediate(oldText);

        // TextMeshProUGUI 컴포넌트 추가
        TextMeshProUGUI tmpText = obj.AddComponent<TextMeshProUGUI>();

        // 기존 Text 속성 유지
        tmpText.text = oldText.text;  // 기존 텍스트 유지
        tmpText.fontSize = oldText.fontSize;  // 폰트 크기 유지
        tmpText.color = oldText.color;  // 색상 유지
        tmpText.alignment = ConvertAlignment(oldText.alignment);  // 정렬 변환
        tmpText.raycastTarget = oldText.raycastTarget;  // 클릭 감지 설정 유지
        tmpText.enableAutoSizing = false;  // 자동 크기 조절 비활성화
        tmpText.fontStyle = ConvertFontStyle(oldText.fontStyle); // Font Style 변환 추가
    }
    private static TextAlignmentOptions ConvertAlignment(TextAnchor anchor)
    {
        switch (anchor)
        {
            case TextAnchor.UpperLeft: return TextAlignmentOptions.TopLeft;
            case TextAnchor.UpperCenter: return TextAlignmentOptions.Top;
            case TextAnchor.UpperRight: return TextAlignmentOptions.TopRight;
            case TextAnchor.MiddleLeft: return TextAlignmentOptions.MidlineLeft;
            case TextAnchor.MiddleCenter: return TextAlignmentOptions.Center;
            case TextAnchor.MiddleRight: return TextAlignmentOptions.MidlineRight;
            case TextAnchor.LowerLeft: return TextAlignmentOptions.BottomLeft;
            case TextAnchor.LowerCenter: return TextAlignmentOptions.Bottom;
            case TextAnchor.LowerRight: return TextAlignmentOptions.BottomRight;
            default: return TextAlignmentOptions.Center;
        }
    }
    private static FontStyles ConvertFontStyle(FontStyle fontStyle)
    {
        switch (fontStyle)
        {
            case FontStyle.Bold:
                return FontStyles.Bold;
            case FontStyle.Italic:
                return FontStyles.Italic;
            case FontStyle.BoldAndItalic:
                return FontStyles.Bold | FontStyles.Italic;
            case FontStyle.Normal:
            default:
                return FontStyles.Normal;
        }
    }
}
#endif