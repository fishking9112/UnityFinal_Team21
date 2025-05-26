using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EvolutionTree : MonoBehaviour
{
    private EvolutionTreeUI evolutionTreeUI;
    public EvolutionTreeUI EvolutionTreeUI => evolutionTreeUI;

    private QueenCondition queenCondition;
    private QueenController queenController;

    [SerializeField] private List<EvolutionNode> evolutionNodeList;
    private Dictionary<int, EvolutionNode> evolutionNodeDic;


    public EvolutionNode selectedNode;

    public void Init(EvolutionTreeUI treeUI)
    {
        evolutionTreeUI = treeUI;

        queenCondition = GameManager.Instance.queen.condition;
        queenController = GameManager.Instance.queen.controller;
        evolutionNodeDic = new Dictionary<int, EvolutionNode>();

        queenCondition.EvolutionPoint.AddAction(evolutionTreeUI.UpdateEvolutionPointText);
        evolutionTreeUI.UpdateEvolutionPointText(queenCondition.EvolutionPoint.Value);

        foreach (EvolutionNode node in evolutionNodeList)
        {
            node.Init(treeUI);
            node.onClickNode = OnClickNodeButton;

            if (!evolutionNodeDic.ContainsKey((int)node.monsterInfoId))
            {
                evolutionNodeDic[(int)node.monsterInfoId] = node;
            }
        }

        SelectFirstNode();
        UpdateAllNode();
    }

    // 진화 버튼을 누르면 진화 확정
    public void OnClickEvolutionButton()
    {
        if (selectedNode == null
            || selectedNode.isUnlock
            || selectedNode.nodeLock)
        {
            return;
        }

        if (queenCondition.EvolutionPoint.Value <= 0)
        {
            // 진화 포인트가 부족하다는 팝업창 있으면 좋을 것 같음
            return;
        }

        selectedNode.isUnlock = true;
        queenCondition.AdjustEvolutionPoint(-1f);

        // 한쪽 노드를 진화시키면 다른 쪽 노드 잠금
        //int parentNodeId = selectedNode.monsterInfo.preNode;

        //foreach (EvolutionNode node in evolutionNodeList)
        //{
        //    if (node.monsterInfo.preNode == parentNodeId && node != selectedNode)
        //    {
        //        node.nodeLock = true;
        //    }
        //}

        UpdateAllNode();
        evolutionTreeUI.UpdateDescriptionWindow(selectedNode);
        evolutionTreeUI?.SetEvolutionButtonState(false);
    }

    // 모든 노드 업데이트
    private void UpdateAllNode()
    {
        foreach (EvolutionNode node in evolutionNodeList)
        {
            bool isActive = ActiveCheck(node);
            node.UpdateButtonState(isActive);
        }
    }

    // 노드를 클릭하면 설명창 내용이 바뀜
    public void OnClickNodeButton(EvolutionNode node)
    {
        selectedNode = node;
        evolutionTreeUI.UpdateDescriptionWindow(node);
        evolutionTreeUI?.SetEvolutionButtonState(!selectedNode.isUnlock);
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
        foreach (var slot in evolutionTreeUI.SlotList)
        {
            if (slot.slotMonsterData == node)
            {
                slot.ClearSlot();
                RemoveQueenSlot(slot.slotIndex);
                return;
            }
        }
    }

    public void SelectFirstNode()
    {
        if (evolutionNodeList == null || evolutionNodeList.Count == 0)
        {
            return;
        }

        selectedNode = evolutionNodeList[0];
        evolutionTreeUI.UpdateDescriptionWindow(selectedNode);
        evolutionTreeUI.SetEvolutionButtonState(!selectedNode.isUnlock);
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
