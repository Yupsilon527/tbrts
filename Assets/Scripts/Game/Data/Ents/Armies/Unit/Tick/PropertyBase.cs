
public class PropertyBase
{
    public DataItemUnit parent;
    public int expiration = 0;
    public int actionInterval = 100;

    public void ExtendCooldown(float cdr = 1)
    {
        BackwardTime((int)(actionInterval * cdr));
    }
    public void SetCooldown(int cooldown)
    {
        expiration = cooldown;
    }
    public virtual bool ForwardTime(int cooldown)
    {
        expiration -= cooldown;
        return expiration <= 0;
    }
    public virtual void BackwardTime(int cooldown)
    {
        expiration += cooldown;
    }
}