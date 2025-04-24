public class QueenActiveSkillSlot : BaseSlot<QueenActiveSkillBase>
{
    public override void AddSlot(int index, QueenActiveSkillBase skill)
    {
        base.AddSlot(index, skill);

        if (index < 0 || index >= slotIcon.Count)
        {
            return;
        }

        slotIcon[index].sprite = DataManager.Instance.iconData.GetSprite(skill.info.icon);
        slotIcon[index].enabled = true;
        slotIcon[index].preserveAspect = true;
    }
}
