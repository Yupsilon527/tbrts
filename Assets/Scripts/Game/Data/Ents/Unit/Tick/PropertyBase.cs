
public abstract class PropertyAction : ITimerAction
{
    public string InternalName = "ERROR";
    public DataItemUnit parent;
    public int nextTime = 0;
    public int actionDelay = 100;
    public int actionInterval = 100;

    public void ExtendCooldown(float cdr = 1)
    {
        Delay((int)(actionInterval * cdr));
    }
    public void SetCooldown(int cooldown)
    {
        nextTime = cooldown;
    }
    public virtual bool ForwardTime(int cooldown)
    {
        nextTime -= cooldown;
        return nextTime <= 0;
    }
    public virtual void Delay(int cooldown)
    {
        nextTime += cooldown;
    }
    public virtual void Reset()
    {
        nextTime = 0;
    }
}
public interface ITimerAction
{
    public void ExtendCooldown(float cdr = 1);
    public void SetCooldown(int cooldown);
    public  bool ForwardTime(int cooldown);
    public  void Delay(int cooldown);
    public  void Reset();
}