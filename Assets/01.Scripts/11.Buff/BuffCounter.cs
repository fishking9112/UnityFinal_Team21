using System;

public class BuffCounter
{
    private int end;
    private int count;
    private Action buffSkillEnd;

    public BuffCounter (int end, Action buffSkillEnd)
    {
        this.end = end;
        this.count = 0;
        this.buffSkillEnd = buffSkillEnd;
    }

    public void BuffEnd()
    {
        count++;
        if(count >= end)
        {
            buffSkillEnd?.Invoke();
        }
    }
}
