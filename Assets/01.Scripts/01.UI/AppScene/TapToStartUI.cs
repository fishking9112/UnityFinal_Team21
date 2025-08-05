using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TapToStartUI : MonoBehaviour
{
    [SerializeField] private GameObject UIGroup;
    [SerializeField] private TextMeshProUGUI tapToStartText;
    private bool isBlinking = false;

    private void Awake()
    {
        UIGroup.SetActive(false);
    }

    private void OnEnable()
    {
        SceneLoadManager.Instance.tapToStartUI = this;
        StartBlinking();
    }

    private void OnDisable()
    {
        StopBlinking();
        UIGroup.SetActive(false);
    }

    public void ActiveUIGroup(bool value)
    {
        UIGroup.SetActive(value);
    }

    private void StartBlinking()
    {
        if (!isBlinking)
        {
            isBlinking = true;
            BlinkTextAlpha().Forget();
        }
    }

    private void StopBlinking()
    {
        isBlinking = false;
    }

    private async UniTaskVoid BlinkTextAlpha()
    {
        float duration = 1.0f; 
        Color originalColor = tapToStartText.color;

        while (isBlinking)
        {
            float t = Mathf.PingPong(Time.unscaledTime / duration, 1f);
            float alpha = Mathf.Lerp(0.2f, 1f, t);

            var newColor = tapToStartText.color;
            newColor.a = alpha;
            tapToStartText.color = newColor;

            await UniTask.Yield(PlayerLoopTiming.Update);
        }

        tapToStartText.color = originalColor;
    }

}
