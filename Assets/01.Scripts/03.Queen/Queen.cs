using UnityEngine;
using UnityEngine.InputSystem;

public class Queen : MonoBehaviour
{
    public QueenCondition condition;
    public QueenController controller;
    public PlayerInput input;

    private void Awake()
    {
        condition = GetComponent<QueenCondition>();
        controller = GetComponent<QueenController>();
        input = GetComponent<PlayerInput>();
    }
}
