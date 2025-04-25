using UnityEngine;

public abstract class QueenActiveSkillBase : MonoBehaviour
{
    public QueenActiveSkillInfo info;
    protected QueenController controller;

    public virtual void Init()
    {
        controller = GameManager.Instance.queen.controller;
    }

    public abstract void UseSkill();
}
