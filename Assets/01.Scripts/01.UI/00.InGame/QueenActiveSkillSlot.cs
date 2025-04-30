public class QueenActiveSkillSlot : BaseSlot<QueenActiveSkillBase>
{
    public override void AddSlot(int index, QueenActiveSkillBase skill)
    {
        base.AddSlot(index, skill);

        if (index < 0 || index >= slotIconList.Count)
        {
            return;
        }

        slotIconList[index].sprite = DataManager.Instance.iconAtlas.GetSprite(skill.info.icon);
        slotIconList[index].enabled = true;
        slotIconList[index].preserveAspect = true;
    }
}
