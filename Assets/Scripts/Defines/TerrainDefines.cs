

public static class TerrainDefines
{


    public static int TileSetWidth = 4;
    public static int TileSetHeight = 4;

    public static int tileFull = 0;
    public static int tileVariationsBegin = 0;
    public static int tileVariationsEnd = 15;

    public static int UnitsPerTile = 1;

    public static int ElevationRaiseChance = 4;

    public static int MapSizeSmall = 30;
    public static int MapSizeMedium = 50;
    public static int MapSizeLarge = 80;
    public enum AreaType
    {
        circle,
        square
    }

    public enum Movement
    {
        Building = -1,
        Boat = 0,
        Swimmer = 1,
        Amphibian = 2,
        Basic = 3,
        GroundFoot = 4,
        GroundMounted = 5,
        GroundWheels = 6,
        Fly = 7,
        Ghost = 8,
        Teleport = 9,
        Total = 10
    };
    public enum Elevation
    {
        Void = 0,
        DeepSea = 1,
        Sea = 2,
        Bridge = 3,
        Swamp = 4,
        Plain = 5,
        Road = 6,
        Forest = 7,
        Hill = 8,
        Mountain = 9,
        Wall = 10,
        City = 11,
        Total = 12
    }

    public static int GetEdgeSprite(bool[] Edges)
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
            return -1;
        }
    }
    public static int GetMoveCost(Movement movement, Elevation Elevation)
    {
        if (CanIWalkOver(movement, Elevation))
        {
            if (Elevation == Elevation.City)
                return 1;
            switch (movement)
            {
                case Movement.Boat:
                    if (Elevation == Elevation.Sea)
                    {
                        return 3;
                    }
                    return 2;
                case Movement.Swimmer:

                    if (Elevation > Elevation.Sea)
                    {
                        return 2;
                    }
                    if (Elevation >= Elevation.Hill)
                    {
                        return 4;
                    }
                    return 3;
                case Movement.Amphibian:
                    if (Elevation >= Elevation.Plain)
                    {
                        return 3;
                    }
                    if (Elevation >= Elevation.Forest)
                    {
                        return 4;
                    }
                    return 2;
                case Movement.Basic:
                    if (Elevation == Elevation.Mountain)
                    {
                        return 3;
                    }
                    if (Elevation == Elevation.Swamp || Elevation >= Elevation.Forest)
                    {
                        return 3;
                    }
                    return 2;
                case Movement.GroundFoot:
                    if (Elevation == Elevation.Mountain)
                    {
                        return 3;
                    }
                    if (Elevation == Elevation.Swamp || Elevation >= Elevation.Forest)
                    {
                        return 3;
                    }
                    if (Elevation == Elevation.Bridge || Elevation == Elevation.Road)
                    {
                        return 1;
                    }
                    return 2;
                case Movement.GroundMounted:
                    if (Elevation == Elevation.Swamp || Elevation == Elevation.Hill)
                    {
                        return 3;
                    }
                    if (Elevation < Elevation.Bridge || Elevation > Elevation.Hill)
                    {
                        return 4;
                    }
                    return 2;
                case Movement.GroundWheels:

                    if (Elevation == Elevation.Bridge || Elevation == Elevation.Road)
                    {
                        return 1;
                    }
                    if (Elevation == Elevation.Swamp)
                    {
                        return 5;
                    }
                    if (Elevation >= Elevation.Forest)
                    {
                        return 4;
                    }
                    return 3;
                case Movement.Fly:

                    if (Elevation == Elevation.Mountain)
                    {
                        return 3;
                    }
                    return 2;
                case Movement.Ghost:
                    if (Elevation == Elevation.Void)
                    {
                        return 2;
                    }
                    return 3;
                case Movement.Teleport:
                    return 4;
            }
        }

        return -1;
    }

    public static bool CanIWalkOver(Movement movement, Elevation elevation)
    {
        switch (movement)
        {
            default:
                return elevation == Elevation.City;
            case Movement.Boat:
                return elevation >= Elevation.DeepSea && elevation <= Elevation.Swamp;
            case Movement.Swimmer:
                return elevation == Elevation.City || elevation >= Elevation.DeepSea &&
                    elevation <= Elevation.Hill;
            case Movement.Amphibian:
                return elevation == Elevation.City || elevation >= Elevation.Sea &&
                    elevation <= Elevation.Hill;
            case Movement.Basic:
            case Movement.GroundFoot:
                return elevation == Elevation.City || (elevation >= Elevation.Swamp &&
                elevation <= Elevation.Mountain);
            case Movement.GroundWheels:
                return elevation == Elevation.City || (elevation >= Elevation.Plain &&
                elevation <= Elevation.Hill);
            case Movement.GroundMounted:
                return elevation == Elevation.City || elevation >= Elevation.Sea &&
                    elevation <= Elevation.Mountain;
            case Movement.Fly:
                return elevation == Elevation.City || elevation < Elevation.Wall;
            case Movement.Ghost:
            case Movement.Teleport:

                return true;
        }
    }
}

