using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MonsterDescriptionUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI summonGaugeText;

    public void ShowUI(Sprite icon, string name, string description, string summonGauge)
    {
        iconImage.sprite = icon;
        nameText.text = name;
        descriptionText.text = description;
        summonGaugeText.text = summonGauge;

        gameObject.SetActive(true);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}
