using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EvolutionNode
{
    public string name;
    public Button button;
    public Image image;
    public string preNodeName;
    public int cost;
    public bool isUnlock;
    public MonsterInfo monsterInfo;
}

public class EvolutionTree : MonoBehaviour
{
    [SerializeField] private List<EvolutionNode> evolutionNodeList;
    private Dictionary<string, EvolutionNode> evolutionNodeDic;

    private void Start()
    {
        evolutionNodeDic = new Dictionary<string, EvolutionNode>();

        foreach(var node in evolutionNodeList)
        {
            evolutionNodeDic[node.name] = node;
            node.button.onClick.AddListener(()=>OnClickNodeButton(node));
        }

        UpdateNode();
    }

    public void OnClickNodeButton(EvolutionNode node)
    {
        node.isUnlock = true;
        UpdateNode();
    }

    public bool ActiveCheck(EvolutionNode node)
    {
        if (string.IsNullOrEmpty(node.preNodeName))
        {
            return true;
        }

        if (evolutionNodeDic.TryGetValue(node.preNodeName,out EvolutionNode preNode))
        {
            return preNode.isUnlock;
        }

        return false;
    }

    private void UpdateButtonStatus(EvolutionNode node)
    {
        bool isActive = ActiveCheck(node);

        if (node.isUnlock && isActive)
        {
            node.image.color = Color.white;
            node.button.interactable = false;
        }
        else if (!node.isUnlock && isActive)
        {
            node.image.color = Color.gray;
            node.button.interactable = true;
        }
        else
        {
            node.image.color = Color.black;
            node.button.interactable = false;
        }
    }

    private void UpdateNode()
    {
        foreach(var node in evolutionNodeList)
        {
            UpdateButtonStatus(node);
        }
    }
}
