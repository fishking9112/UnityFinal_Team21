using UnityEngine;
using UnityEngine.InputSystem;

public class Queen : MonoBehaviour
{
    public QueenCondition condition;
    public QueenController controller;
    public QueenActiveSkillManager queenActiveSkillManager;
    public PlayerInput input;

    private void Awake()
    {
        GameManager.Instance.queen = this;
        condition = GetComponent<QueenCondition>();
        controller = GetComponent<QueenController>();
        queenActiveSkillManager = GetComponent<QueenActiveSkillManager>();
        input = GetComponent<PlayerInput>();

        SlotChange slotChange = FindObjectOfType<SlotChange>();

        if (slotChange != null)
        {
            slotChange.Init(controller, input);
        }
    }
}
