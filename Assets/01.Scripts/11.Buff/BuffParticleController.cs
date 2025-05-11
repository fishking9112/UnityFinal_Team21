using System;

public class BuffParticleController
{
    private int end;
    private int count;
    private bool isRemove;

    private Action removeBuffParticle;

    public BuffParticleController(int end, Action removeBuffParticle)
    {
        this.end = end;
        this.count = 0;
        this.isRemove = false;
        this.removeBuffParticle = removeBuffParticle;
    }

    public void RemoveParticle()
    {
        if (isRemove)
        {
            return;
        }

        count++;
        if(count >= end)
        {
            isRemove = true;
            removeBuffParticle?.Invoke();
        }
    }

    public void ForceRemoveParticle()
    {
        if (isRemove)
        {
            return;
        }

        isRemove = true;
        removeBuffParticle?.Invoke();
    }
}
