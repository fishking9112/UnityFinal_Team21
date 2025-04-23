public class QueenActiveSkillSlotUI : BaseSlotUI<QueenActiveSkillBase>
{
    public override void AddSlot(int index, QueenActiveSkillBase skill)
    {
        base.AddSlot(index, skill);

        if (index < 0 || index >= slotIcon.Count)
        {
            return;
        }

        slotIcon[index].sprite = DataManager.Instance.iconData.GetSprite(skill.outfit);
        slotIcon[index].enabled = true;
        slotIcon[index].preserveAspect = true;
    }
}
