using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEventProgressMirrorUI : MonoBehaviour
{
    [SerializeField] private GameEventProgressUI sourceUI;
    [SerializeField] private Image eventIconPrefab;
    [SerializeField] private RectTransform background;
    [SerializeField] private Image progressBarFill;

    private readonly Dictionary<ScheduledEvent, Image> mirroredIcons = new();

    private void OnEnable()
    {
        SetProgressBar();
        SetIcons();
    }

    private void SetProgressBar()
    {
        progressBarFill.fillAmount = sourceUI.GetProgressAmount();
    }

    private void SetIcons()
    {
        foreach (var scheduled in sourceUI.GetScheduledEvents())
        {
            if (!mirroredIcons.ContainsKey(scheduled))
            {
                Image icon = Instantiate(eventIconPrefab, background);
                if (!string.IsNullOrEmpty(scheduled.eventInfo.icon))
                {
                    icon.sprite = DataManager.Instance.iconAtlas.GetSprite(scheduled.eventInfo.icon);
                }

                icon.GetComponent<RectTransform>().anchoredPosition =
                    GetIconSpawnPosition(scheduled.triggerProgress);

                mirroredIcons.Add(scheduled, icon);
            }
        }

        List<ScheduledEvent> toRemove = new();
        foreach (var pair in mirroredIcons)
        {
            if (!sourceUI.IsEventScheduled(pair.Key))
            {
                pair.Value.gameObject.SetActive(false);
                toRemove.Add(pair.Key);
            }
        }

        foreach (var key in toRemove)
        {
            mirroredIcons.Remove(key);
        }
    }

    private Vector2 GetIconSpawnPosition(float fillAmount)
    {
        float width = background.rect.width;
        float x = (fillAmount - 0.5f) * width;
        return new Vector2(x, 0f);
    }
}
