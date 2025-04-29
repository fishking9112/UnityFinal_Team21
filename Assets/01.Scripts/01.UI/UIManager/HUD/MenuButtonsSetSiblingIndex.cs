using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuButtonsSetSiblingIndex : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MenuHUD menuHUD;
    public Transform Panel;
   // private Transform parentObj;

    private void Awake()
    {
       // parentObj = transform.parent; 
        transform.GetChild(1)   .GetComponent<Image>().alphaHitTestMinimumThreshold = 0.2f;
    }

    // 마우스가 오버됐을 때 호출
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("마우스가 위에 올라왔습니다.");
        menuHUD.BlackBackground.SetAsLastSibling();
        menuHUD.BlackBackground.gameObject.SetActive(true);
        transform.SetAsLastSibling();
       // parentObj.SetAsLastSibling();
        Panel.gameObject.SetActive(true);

        // 여기에 오버됐을 때 동작할 코드를 작성
    }

    // 마우스가 나갔을 때 호출
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("마우스가 빠져나갔습니다.");
        menuHUD.BlackBackground.SetAsFirstSibling();
        menuHUD.BlackBackground.gameObject.SetActive(false);
        Panel.gameObject.SetActive(false);
        // 여기에 나갔을 때 동작할 코드를 작성 
    }
}
