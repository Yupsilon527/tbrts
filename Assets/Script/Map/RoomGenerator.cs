using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public void GenerateWorldFromData(MapData map)
    {
        DataItemWorld.main.GenerateWorld(map);
        DataItemWorld.main.FinalizeWorld();
        DisplayItemWorld.main.GenerateWhole();   
    }
}
