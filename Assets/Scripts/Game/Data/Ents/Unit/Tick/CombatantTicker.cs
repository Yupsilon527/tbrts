
public interface CombatantTicker
{
    REVISE
    public virtual void Tick(int steps)
    {
    }
    public virtual int GetNextTick(int ticks)
    {
        return int.MaxValue;
    }
}
