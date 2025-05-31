using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RemainPointDescriptionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject descriptionUI;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Button btn;
    [SerializeField] private GameHUD gameHUD;

    private void Awake()
    {
        descriptionUI.SetActive(false);
        titleText.text = "진화 포인트";
        descText.text = "사용하지 않은 진화 포인트가 있습니다.\n클릭하여 사용해보세요.";
        btn.onClick.AddListener(OnClickUI);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        descriptionUI.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        descriptionUI.SetActive(false);
    }
    
    public void OnClickUI()
    {
        descriptionUI.SetActive(false);
        gameHUD.ShowWindow<EvolutionTreeUI>();
    }
}
