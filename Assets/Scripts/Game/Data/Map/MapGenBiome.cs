
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
        Initalize(map.MapData.GetHeight(), map.MapData.GetWidth());
        for (int iY = 0; iY < map.MapData.GetHeight(); iY++)
        {
            for (int iX = 0; iX < map.MapData.GetWidth(); iX++)
            {
                SetTile(iX, iY, map.MapData.GetTileAt(iX, iY, biomeData));
            }
        }
    }
}
