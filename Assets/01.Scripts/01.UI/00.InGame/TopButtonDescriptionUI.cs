using TMPro;
using UnityEngine;

public class TopButtonDescriptionUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    public void ShowUI(string name, string description)
    {
        nameText.text = name;
        descriptionText.text = description;

        gameObject.SetActive(true);
    }

    public void HideUI()
    {
        gameObject.SetActive(false);
    }
}
