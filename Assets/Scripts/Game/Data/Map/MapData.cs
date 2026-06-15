using System;
using UnityEngine;

public class MapData {

    public string[] building_data;  //obsolete?
    public ObjectData[] object_data = new ObjectData[0];

    public MapBiomeSO biome;
    public ElevationData[] elevation_data;
    public SidewaysTile[,] tile_data;

    public SidewaysTile GetTileAt(Vector2Int pos)
    {
        if (pos.y < 0 || pos.x < 0 || pos.y >= tile_data.GetLength(0) || pos.x >= tile_data.GetLength(1))
        { return new SidewaysTile(); }
        return tile_data[pos.y, pos.x];
    }
    
    public int GetWidth()
    {
        return tile_data.GetLength(1);
    }
    public int GetHeight()
    {
        return tile_data.GetLength(0);
    }

    public bool isOnMap(int x, int y)
    {
        return (x >= 0 && x < GetWidth() && y >= 0 && y < GetHeight());
    }

    /*
	public bool Save(GameResource game,bool Overwrite)
	{
		GameDirectory.CleanseString (Name);
		FilterEntities (game);
		Version=Game.Version;

		if (GetRuleset () == MapType.FairSkirmish) {
			World = "Random";
		} else {
			World = game.activeWorld.FileName;
		}

		string name = GameDirectory.CleanseString (Name);
		string path=Application.dataPath+GameDirectory.SaveDataFolder+GameDirectory.MapFile + name + GameDirectory.MapFileExtension;
		if (Game.PrechaceEverything) {
			path=Application.dataPath+GameDirectory.SaveDataFolder+GameDirectory.MapFile + name + GameDirectory.DataFileExtension;
		}

		if ((File.Exists (path) == true && Overwrite == true) ||
			(File.Exists (path) == false)) {			
			var serializer = new XmlSerializer (typeof(MapData));
			FileStream stream = File.Create (path);		
			serializer.Serialize (stream, this);

			stream.Close ();

			return true;
		} else {
			return false;
		}
	}
    */
    public int[] GetSize()
	{
        return new int[] { tile_data.GetLength(0), tile_data.GetLength(1) } ;
	}

    public void InitElevationData()
    {
        for (int iE=0; iE<elevation_data.Length; iE++)
        {
            elevation_data[iE].elevationID = iE;
        }
    }
}

public class ObjectData
{
    public ObjectDataSO data;
    public bool FacesRight;
    public Vector2Int GridLocation;
}