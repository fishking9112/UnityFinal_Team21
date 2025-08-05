using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class MonsterDescriptionUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public LocalizeStringEvent nameLocalize;
    public TextMeshProUGUI descriptionText;
    public LocalizeStringEvent descriptionLocalize;
    public TextMeshProUGUI summonGaugeText;

    public void ShowUI(Sprite icon, string name, string description, string summonGauge)
    {
        iconImage.sprite = icon;
        StringManager.Instance.SetString(name, nameLocalize);
        StringManager.Instance.SetString(description, descriptionLocalize);
        summonGaugeText.text = summonGauge;

        gameObject.SetActive(true);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}
