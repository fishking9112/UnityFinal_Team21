using UnityEngine;

public class Queen : MonoBehaviour
{
    public QueenCondition condition;
    public QueenController controller;

    private void Awake()
    {
        GameManager.Instance.queen = this;
        condition = GetComponent<QueenCondition>();
        controller = GetComponent<QueenController>();
    }
}
