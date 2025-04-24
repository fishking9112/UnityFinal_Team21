using UnityEngine;
using UnityEngine.UI;


public class PauseUI : MonoBehaviour
{
    [Header("패널들")]
    public GameObject optionPanel;

    [Header("UI 버튼들")]
    public Button continueButton;
    public Button optionButton;
    public Button exitButton;


    public void SetOptionPanel(bool active)
    {
        optionPanel?.SetActive(active);
    }

}
