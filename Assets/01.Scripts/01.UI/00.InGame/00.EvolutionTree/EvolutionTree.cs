using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class EvolutionTree : MonoBehaviour
{
    [SerializeField] private List<EvolutionNode> evolutionNodeList;
    private Dictionary<int, EvolutionNode> evolutionNodeDic;

    [Header("UI")]
    [SerializeField] private Image descriptionImage;
    [SerializeField] private Button evolutionButton;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI costText;

    [Header("Slot")]
    [SerializeField] private List<EvolutionSlot> slotList;

    public EvolutionNode selectedNode;

    private void Start()
    {
        evolutionNodeDic = new Dictionary<int, EvolutionNode>();

        foreach (EvolutionNode node in evolutionNodeList)
        {
            node.Init(this);

            if (!evolutionNodeDic.ContainsKey(node.monsterInfoId))
            {
                evolutionNodeDic[node.monsterInfoId] = node;
            }

            evolutionButton.onClick.AddListener(OnClickEvolutionButton);
            evolutionButton.gameObject.SetActive(false);
        }

        foreach(EvolutionSlot slot in slotList)
        {
            slot.evolutionTree = this;
        }

        descriptionImage.enabled = false;
        description.text = string.Empty;
        healthText.text = string.Empty;
        costText.text = string.Empty;
        attackText.text = string.Empty;
    }


    // 진화 버튼을 누르면 진화 확정
    public void OnClickEvolutionButton()
    {
        if(selectedNode == null
            || selectedNode.isUnlock
            || selectedNode.nodeLock)
        {
            return;
        }

        selectedNode.isUnlock = true;

        // 한쪽 노드를 진화시키면 다른 쪽 노드 잠금
        int parentNodeId = selectedNode.monsterInfo.preNode;
        
        foreach(EvolutionNode node in evolutionNodeList)
        {
            if(node.monsterInfo.preNode == parentNodeId && node != selectedNode)
            {
                node.nodeLock = true;
            }
        }

        UpdateAllNode();
        UpdateDescriptionWindow(selectedNode);
        evolutionButton.gameObject.SetActive(false);
    }

    // 모든 노드 업데이트
    private void UpdateAllNode()
    {
        foreach(EvolutionNode node in evolutionNodeList)
        {
            node.UpdateButtonState();
        }
    }

    // 노드를 클릭하면 설명창 내용이 바뀜
    public void OnClickNodeButton(EvolutionNode node)
    {
        selectedNode = node;
        UpdateDescriptionWindow(node);

        evolutionButton.gameObject.SetActive(!selectedNode.isUnlock);
    }

    // 설명창 초기화
    private void UpdateDescriptionWindow(EvolutionNode node)
    {
        MonsterInfo info = node.monsterInfo;

        descriptionImage.enabled = true;
        descriptionImage.sprite = node.image.sprite;
        description.text = info.description;
        healthText.text = $"체력 : {info.health}";
        attackText.text = $"공격력 : {info.attack}";
        costText.text = $"소환 비용 : {info.cost}";
    }

    // 노드가 활성화 되어 있는 지 확인 (해금된 노드 or 해금할 수 있는 노드)
    public bool ActiveCheck(EvolutionNode node)
    {
        if (node.monsterInfo.preNode == -1)
        {
            return true;
        }

        if (evolutionNodeDic.TryGetValue(node.monsterInfo.preNode, out var preNode))
        {
            return preNode.isUnlock;
        }

        return false;
    }

    // 이전에 등록된 슬롯 데이터 제거 (몬스터 A를 1번칸에 등록한 상태로 2번칸에 등록하려할 때 1번칸의 데이터를 없애주는 역할)
    public void RemovePreSlotData(EvolutionNode node)
    {
        foreach (var slot in slotList)
        {
            if (slot.FindPreNode(node))
            {
                slot.ClearSlot();
                return;
            }
        }
    }
}
