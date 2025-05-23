public interface IBuffStrategy
{
    void Apply(BaseController target, Buff buff, BuffInfo info, float amount);
    void Remove(BaseController target, Buff buff);
}