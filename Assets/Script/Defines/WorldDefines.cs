using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldDefines
{
    public static int SearchIndex = 0;
    public enum Elevations
    {
        Void,
        DeepSea,
        Sea,
        Swamp,
        Plain,
        Forest,
        Hill,
        Mountain,
        Wall
    }
    public enum Movement
    {
        NoMovement = -1,
        Sea = 0,
        Swimmer = 1,
        Amphibian = 2,
        Wheel = 3,
        Foot = 4,
        Fly = 5,
        Space = 6
    };
    public static bool CanFitEntity(DataItemTile t, Mob e)
    {
        if (!t.IsPassible())
            return false;
        if (e != null)
            switch (e.GetEntitySize())
            {
                case Mob.EntitySize.small:
                    return t.EntityCheck(e);
                case Mob.EntitySize.medium:
                    bool passible = t.EntityCheck(e);
                    if (passible && (!t.neighbors[0].IsPassible() || !t.neighbors[0].EntityCheck(e)))
                    {
                        passible = false;
                    }
                    if (passible && (!t.neighbors[1].IsPassible() || !t.neighbors[1].EntityCheck(e)))
                    {
                        passible = false;
                    }
                    return passible;
                case Mob.EntitySize.large:
                    passible = t.EntityCheck(e);
                    if (passible)
                    {
                        foreach (var n in t.neighbors)
                        {
                            if (!n.IsPassible() || !n.EntityCheck(e))
                            {
                                passible = false;
                                break;
                            }
                        }
                    }
                    return passible;
            }
        return t.IsPassible() && t.EntityCheck(e);
    }
    public static bool CanWalkOver(Mob e, DataItemTile t)
    {
        throw new NotImplementedException();
    }
        public static bool CanWalkOver(Movement movement, Elevations Elevation)
    {
        switch (movement)
        {

            case Movement.NoMovement:
                return false;
            case Movement.Wheel:

                return !(Elevation == Elevations.DeepSea ||
                    (Elevation == Elevations.Sea) ||
                Elevation == Elevations.Void ||
                Elevation == Elevations.Wall ||
                Elevation == Elevations.Mountain);

            case Movement.Foot:

                return !(Elevation == Elevations.DeepSea ||
                    (Elevation == Elevations.Sea) ||
                    Elevation == Elevations.Void ||
                    Elevation == Elevations.Wall);

            case Movement.Swimmer:
            case Movement.Amphibian:

                return !(Elevation == Elevations.Void ||
                    Elevation == Elevations.Wall ||
                    Elevation == Elevations.Mountain);

            case Movement.Fly:

                return !(Elevation == Elevations.Wall);

            case Movement.Space:

                return true;

            case Movement.Sea:

                return (Elevation == Elevations.DeepSea || Elevation == Elevations.Sea);



        }
        return false;
    }
    public static int GetMoveCost(Movement movement, Elevations Elevation)
    {
        if (CanWalkOver(movement, Elevation))
        {

            switch (movement)
            {
                case Movement.Wheel:
                    if (Elevation == Elevations.Forest)
                    {
                        return 3;
                    }
                    if (Elevation == Elevations.Swamp)
                    {
                        return 3;
                    }
                    if (Elevation == Elevations.Hill)
                    {
                        return 3;
                    }
                    return 2;
                case Movement.Foot:
                    if (Elevation == Elevations.Mountain)
                    {
                        return 4;
                    }
                    if (Elevation == Elevations.Swamp)
                    {
                        return 3;
                    }
                    if (Elevation == Elevations.Hill)
                    {
                        return 3;
                    }
                    return 2;
                case Movement.Amphibian:

                    if (Elevation == Elevations.Hill)
                    {
                        return 3;
                    }
                    if (Elevation == Elevations.DeepSea)
                    {
                        return 3;
                    }
                    return 2;
                case Movement.Swimmer:

                    if (Elevation > Elevations.Sea)
                    {
                        return 3;
                    }
                    return 2;
                case Movement.Sea:

                    if (Elevation == Elevations.Sea)
                    {
                        return 3;
                    }
                    return 2;
                case Movement.Fly:

                    if (Elevation == Elevations.Mountain)
                    {
                        return 3;
                    }
                    return 2;
                case Movement.Space:

                    if (Elevation == Elevations.Void)
                    {
                        return 2;
                    }
                    return 3;
            }
        }

        return -1;
    }
}
