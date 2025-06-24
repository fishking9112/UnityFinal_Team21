using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class TopButtonDescriptionUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public LocalizeStringEvent titleLocalize;
    public TextMeshProUGUI descriptionText;
    public LocalizeStringEvent descriptionLocalize;

    public void ShowUI(string name, string description)
    {
        StringManager.Instance.SetString(name, titleLocalize);
        StringManager.Instance.SetString(description, descriptionLocalize);

        gameObject.SetActive(true);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}
