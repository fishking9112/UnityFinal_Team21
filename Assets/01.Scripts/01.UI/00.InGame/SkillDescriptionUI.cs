using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillDescriptionUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI coolTimeText;
    public TextMeshProUGUI manaText;

    public void ShowUI(Sprite icon, string name, string description, string coolTime, string mana)
    {
        iconImage.sprite = icon;
        nameText.text = name;
        descriptionText.text = description;
        coolTimeText.text = coolTime;
        manaText.text = mana;

        gameObject.SetActive(true);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}
