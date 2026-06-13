using UnityEngine;

[CreateAssetMenu(fileName = "Map Biome", menuName = "GameData/Map Generation/Map Biome")]
public class MapBiomeSO : ScriptableObject
{
    public ElevationData[] Elevations;
    public float ObjectDensity = 1;
    public ElevationData GetElevation(char e)
    {
        foreach (var item in Elevations)
        {
            if (item.CharID == e)
            {
                return item;
            }
        }
        return null;
    }
    public ElevationData Ground { get { return GetElevation('-'); } }
}
