using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionNode : MonoBehaviour
{
    [Header("UI")]
    public Image image;
    [SerializeField] private Button button;
    [SerializeField] private TextMeshProUGUI nameText;

    public bool isUnlock;
    public bool nodeLock;
    public IDMonster monsterInfoId;
    public MonsterInfo monsterInfo;

    private EvolutionTree evolutionTree;

    public void Init(EvolutionTree tree)
    {
        evolutionTree = tree;

        if(DataManager.Instance.monsterDic.TryGetValue((int)monsterInfoId,out var info))
        {
            monsterInfo = info;
            image.sprite = DataManager.Instance.iconData.GetSprite(monsterInfo.outfit);
        }
        else
        {
            Utils.Log($"{monsterInfoId}의 info는 존재하지 않습니다.");
        }

        button.onClick.AddListener(OnClickNode);
        UpdateButtonState();
    }

    public void OnClickNode()
    {
        evolutionTree.OnClickNodeButton(this);
    }

    // 현재 버튼(노드)들의 상태 업데이트
    public void UpdateButtonState()
    {
        if (nodeLock)
        {
            button.interactable = false;
            image.color = Color.red;
            nameText.text = "잠김";
            return;
        }

        bool isActive = evolutionTree.ActiveCheck(this);

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
}
