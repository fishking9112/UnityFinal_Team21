using UnityEngine;

public class Queen : MonoBehaviour
{
    private QueenCondition condition;
    private QueenController controller;

    private void Awake()
    {
        condition = GetComponent<QueenCondition>();
        controller = GetComponent<QueenController>();
    }
}
