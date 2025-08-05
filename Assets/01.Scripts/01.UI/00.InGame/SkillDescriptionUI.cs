using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class SkillDescriptionUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public LocalizeStringEvent nameLocalize;
    public TextMeshProUGUI descriptionText;
    public LocalizeStringEvent descriptionLocalize;
    public TextMeshProUGUI coolTimeText;
    public TextMeshProUGUI manaText;

    public void ShowUI(Sprite icon, string name, string description, string coolTime, string mana)
    {
        iconImage.sprite = icon;
        StringManager.Instance.SetString(name, nameLocalize);
        StringManager.Instance.SetString(description, descriptionLocalize);
        coolTimeText.text = coolTime;
        manaText.text = mana;

        gameObject.SetActive(true);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}
