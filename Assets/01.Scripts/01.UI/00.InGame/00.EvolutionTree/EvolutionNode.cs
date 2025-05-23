using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EvolutionNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image image;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private GameObject selectedUI;
    [SerializeField] private EvolutionTreeUI evolutionTreeUI;

    public bool isUnlock;
    public bool nodeLock;
    public IDMonster monsterInfoId;
    public MonsterInfo monsterInfo;

    public UnityAction<EvolutionNode> onClickNode;

    public void Init(EvolutionTreeUI treeUI)
    {
        evolutionTreeUI = treeUI;

        selectedUI = transform.Find("Select")?.gameObject;

        if (DataManager.Instance.monsterDic.TryGetValue((int)monsterInfoId,out var info))
        {
            monsterInfo = info;
            image.sprite = DataManager.Instance.iconAtlas.GetSprite(monsterInfo.outfit);
        }

        selectedUI.SetActive(false);
        button.onClick.AddListener(() => onClickNode?.Invoke(this));
    }

    // 현재 버튼(노드)들의 상태 업데이트
    public void UpdateButtonState(bool isActive)
    {
        if (nodeLock)
        {
            button.interactable = false;
            image.color = Color.red;
            nameText.text = "잠김";
            return;
        }

        // 해금 된 노드
        if (isUnlock)
        {
            image.color = Color.white;
            nameText.text = monsterInfo.name;
            button.interactable = true;
        }
        else
        {
            // 해금 안된 것 중 해금할 수 있는 노드
            if (isActive)
            {
                image.color = Color.gray;
                nameText.text = monsterInfo.name;
                button.interactable = true;
            }
            // 아직 해금할 수 없는 노드
            else
            {
                image.color = Color.black;
                nameText.text = "???";
                button.interactable = false;
            }
        }
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isUnlock)
            evolutionTreeUI.EvolutionDragIcon.OnBeginDrag();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isUnlock)
            evolutionTreeUI.EvolutionDragIcon.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isUnlock)
            evolutionTreeUI.EvolutionDragIcon.OnEndDrag();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (isUnlock)
        {
            evolutionTreeUI.PassEvolutionNodeInfo(this);
            evolutionTreeUI.tfEvolutionDragIcon.position = transform.position;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(isUnlock)
            selectedUI.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isUnlock)
            selectedUI.SetActive(false);
    }
}
