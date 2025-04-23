using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class EvolutionTree : MonoBehaviour
{
    private QueenCondition condition;

    [SerializeField] private List<EvolutionNode> evolutionNodeList;
    private Dictionary<int, EvolutionNode> evolutionNodeDic;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI evolutionPointText;
    [SerializeField] private Image descriptionImage;
    [SerializeField] private Button evolutionButton;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI costText;

    [Header("Slot")]
    [SerializeField] private List<EvolutionSlot> slotList;

    public EvolutionNode selectedNode;
    private QueenController queenController;

    private void Awake()
    {
        condition = GameManager.Instance.queen.condition;
        queenController = GameManager.Instance.queen.controller;
        evolutionNodeDic = new Dictionary<int, EvolutionNode>();

        condition.EvolutionPoint.AddAction(UpdateEvolutionPointText);
        UpdateEvolutionPointText(condition.EvolutionPoint.Value);

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
    }

    private void OnEnable()
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            slotList[i].evolutionTree = this;
            slotList[i].slotIndex = i;
        }

        selectedNode = evolutionNodeList[0];
        UpdateDescriptionWindow(evolutionNodeList[0]);
    }

    private void UpdateEvolutionPointText(float evolutionPoint)
    {
        evolutionPointText.text = $"Ep. {evolutionPoint}";
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

        if(condition.EvolutionPoint.Value <= 0)
        {
            // 진화 포인트가 부족하다는 팝업창 있으면 좋을 것 같음
            return;
        }

        selectedNode.isUnlock = true;
        condition.AdjustEvolutionPoint(-1f);

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
        healthText.text = $"기본 체력 : {info.health}";
        attackText.text = $"기본 공격력 : {info.attack}";
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
            if (slot.slotMonsterData == node)
            {
                slot.ClearSlot();
                RemoveQueenSlot(slot.slotIndex);
                return;
            }
        }
    }

    // 진화 트리 슬롯에 등록한 몬스터를 퀸 슬롯에도 등록
    public void AddQueenSlot(MonsterInfo monster, int index)
    {
        queenController.monsterSlot.AddSlot(index, monster);
    }

    // 진화 트리 슬롯에 제거한 몬스터를 퀸 슬롯에도 제거
    public void RemoveQueenSlot(int index)
    {
        queenController.monsterSlot.RemoveSlot(index);
    }
}
