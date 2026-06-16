
public class DataItem
{
    public static int GlobalEntityId = 0;
    public int eID = 0;
    public DataItem()
    {
        eID = GlobalEntityId++;
    }
}
