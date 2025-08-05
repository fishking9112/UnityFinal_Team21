using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EvolutionTree : MonoBehaviour
{
    private EvolutionTreeUI evolutionTreeUI;
    public EvolutionTreeUI EvolutionTreeUI => evolutionTreeUI;

    private QueenCondition queenCondition;

    [SerializeField] private List<EvolutionNode> evolutionNodeList;
    private Dictionary<int, EvolutionNode> evolutionNodeDic;


    public EvolutionNode selectedNode;

    public void Init(EvolutionTreeUI treeUI)
    {
        evolutionTreeUI = treeUI;

        queenCondition = GameManager.Instance.queen.condition;
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
            // 진화 포인트가 부족하다는 팝업창 있으면 좋을 것 같음 <= 부족시 진화 버튼 텍스트 변경
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
}
