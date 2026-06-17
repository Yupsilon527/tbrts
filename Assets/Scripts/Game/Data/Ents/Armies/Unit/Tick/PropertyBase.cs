
public abstract class PropertyAction
{
    public string InternalName = "ERROR";
    public DataItemUnit parent;
    public int expiration = 0;
    public int actionDelay = 100;
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
    public void Reset()
    {
        expiration = 0; 
        uses = 0;
    }
    }