
public class DataItem
{
    public static int GlobalEntityId = 0;
    public int eID = 0;
    public string InternalName;
    public DataItem()
    {
        eID = GlobalEntityId++;
    }
}
