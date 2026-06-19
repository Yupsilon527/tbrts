

public static class TerrainDefines
{


    public static int TileSetWidth = 4;
    public static int TileSetHeight = 4;

    public static int tileFull = 0;
    public static int tileVariationsBegin = 0;
    public static int tileVariationsEnd = 15;

    public static int UnitsPerTile = 2;

    public static int ElevationRaiseChance = 4;

    public static int MapSizeSmall = 30;
    public static int MapSizeMedium = 50;
    public static int MapSizeLarge = 80;
    public enum AreaType
    {
        circle,
        square
    }
    public enum Elevation
    {
        Void = 0,
        DeepSea = 1,
        Sea = 2,
        Swamp = 3,
        Bridge = 4,
        Plain = 5,
        Road = 6,
        Forest = 7,
        Hill = 8,
        Mountain = 9,
        Wall = 10,
        City = 11,
    }

    public enum Movement
    {
        NoMovement = -1,
        Sea = 0,
        Swimmer = 1,
        Amphibian = 2,
        Ground = 3,
        GroundVersatile = 4,
        Fly = 5,
        Ghost = 6,
        Teleport = 6
    };

    public static int GetSprite(bool[] Edges)
    {

        //0 - 1
        //- x -
        //2 - 3
        if (
            Edges[0] &&
            Edges[1] &&
            Edges[2] &&
            Edges[3]
        )
        {
            return 0;
        }
        else
            if (
                Edges[0] &&
                Edges[1] &&
                !Edges[2] &&
                !Edges[3]
            )
        {
            return 1;
        }
        else
                if (
                    Edges[0] &&
                    !Edges[1] &&
                    Edges[2] &&
                    !Edges[3]
                )
        {
            return 2;
        }
        else
                    if (
                        !Edges[0] &&
                        !Edges[1] &&
                        Edges[2] &&
                        Edges[3]
                    )
        {
            return 3;
        }
        else
                        if (
                            !Edges[0] &&
                             Edges[1] &&
                            !Edges[2] &&
                             Edges[3]
                        )
        {
            return 4;
        }
        else
                            if (
                                Edges[0] &&
                                Edges[1] &&
                                !Edges[2] &&
                                Edges[3]
                            )
        {
            return 5;
        }
        else
                                if (
                                    Edges[0] &&
                                    Edges[1] &&
                                    Edges[2] &&
                                    !Edges[3]
                                )
        {
            return 6;
        }
        else
                                    if (
                                        !Edges[0] &&
                                        Edges[1] &&
                                        Edges[2] &&
                                        Edges[3]
                                    )
        {
            return 7;
        }
        else
                                        if (
                                            Edges[0] &&
                                            !Edges[1] &&
                                            Edges[2] &&
                                            Edges[3]
                                        )
        {
            return 8;
        }
        else
                                            if (
                                                !Edges[0] &&
                                                !Edges[1] &&
                                                 Edges[2] &&
                                                !Edges[3]
                                            )
        {
            return 9;
        }
        else
                                                if (
                                                    !Edges[0] &&
                                                    !Edges[1] &&
                                                    !Edges[2] &&
                                                     Edges[3]
                                                )
        {
            return 10;
        }
        else
                                                    if (
                                                         Edges[0] &&
                                                        !Edges[1] &&
                                                        !Edges[2] &&
                                                        !Edges[3]
                                                    )
        {
            return 11;
        }
        else
                                                        if (
                                                            !Edges[0] &&
                                                             Edges[1] &&
                                                            !Edges[2] &&
                                                            !Edges[3]
                                                        )
        {
            return 12;
        }
        else
                                                            if (
                                                                !Edges[0] &&
                                                                Edges[1] &&
                                                                Edges[2] &&
                                                                !Edges[3]
                                                            )
        {
            return 14;
        }
        else
                                                                if (
                                                                    Edges[0] &&
                                                                    !Edges[1] &&
                                                                    !Edges[2] &&
                                                                    Edges[3]
                                                                )
        {
            return 13;
        }
        else
        {
            UnityEngine.Debug.Log(Edges[0] + " " + Edges[1] + " " + Edges[2] + " " + Edges[3]);
            return -1;
        }
    }
}

