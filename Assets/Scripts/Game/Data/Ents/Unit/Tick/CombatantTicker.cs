
public interface CombatantTicker
{
    public abstract bool Tick(int time);
    public abstract int GetNextTick();
}
