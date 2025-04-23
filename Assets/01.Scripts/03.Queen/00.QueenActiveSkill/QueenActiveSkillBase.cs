using UnityEngine;

public abstract class QueenActiveSkillBase : MonoBehaviour
{
    public int id;
    public string skillName;
    public float cost;
    public string outfit;

    protected QueenController controller;

    protected virtual void Start()
    {
        controller = GameManager.Instance.queen.controller;
    }

    public abstract void UseSkill();
}
