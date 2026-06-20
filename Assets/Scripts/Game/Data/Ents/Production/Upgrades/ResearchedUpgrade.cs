
public class ResearchedUpgrade
{
    public TechData upgrade;
    public int level;
    public int maxes;

    public ResearchedUpgrade(TechData upgrade) : this(upgrade, 1, 1)
    {
    }
    public ResearchedUpgrade(TechData upgrade, int level, int maxes)
    {
        this.upgrade = upgrade;
        this.level = level;
        this.maxes = maxes;
    }

    public override string ToString()
    {
        return $"{upgrade.InternalName} level ({level}/{maxes})";
    }
}