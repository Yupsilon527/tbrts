
public class MapGenBiome : MapGen
{
    public override void GenerateMap()
    {
        GenerateMapFromBiomeData(biomeData, mapData);
        SidewaysMap.main.DrawTheMapFromEditorData(OutputToMapData());
    }
    public void GenerateMapFromBiomeData( MapBiomeSO biome, MapChunkSO map)
    {
        biomeData = biome;
        Initalize(map.GetHeight(), map.GetWidth());
        for (int iY = 0; iY < map.GetHeight(); iY++)
        {
            for (int iX = 0; iX < map.GetWidth(); iX++)
            {
                SetTile(iX, iY, map.GetTileAt(iX, iY, biomeData));
            }
        }
    }
}
