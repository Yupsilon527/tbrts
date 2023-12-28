using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileRayScan
{
    public enum HitScanBehavior //TODO
    {
        Ghost = 0,       //ignores all collisions
        Line = 1,       //halts when hitting wall
        Light = 2,    //similar to stop, but can proceed once if the target is inside building
        Throw = 3,   //x or y speed gets reduced to 0
        Grounded = 4,   //only moves if tile is ground
    }
    public class Trajectory
    {
        public float minrange = 0;
        public float maxrange = 0;
        public HitScanBehavior behavior = HitScanBehavior.Ghost;
        public bool stop_on_object = false;
        public bool always_travel_max_distance = false;

        public enum ActorBehavior
        {
            never_stop,
            stop_allies,
            stop_enemies,
            stop_all
        }
        public Mob launcher = null;
        public ActorBehavior stop_on_actor = ActorBehavior.never_stop;
        public int PierceActors = 1;
    }
    public class Result
    {
        public DataItemTile passible_location;
        public DataItemTile[] trajectory = new DataItemTile[0];

        public DataItemTile GetLastTile()
        {
            if (trajectory.Length > 0)
                return trajectory[trajectory.Length - 1];
            return null;
        }
        public DataItemTile GetLastPassibleTile()
        {
            return passible_location;
        }
    }

    /*public static Result HitScanDirection(Vector2 origin, Vector2 dir, Trajectory data)
    {
        if (dir.sqrMagnitude == 0)
            return new Result();
        Vector2 dest = (origin + dir.normalized * data.maxrange);
        dir.y *= -1;
        return HitScanPoint(origin, dest, data);
    }*/
    public static Result HitScanPoint(HexCoords origin, HexCoords end, Trajectory data)
    {

        Result hitData = new Result();

        Vector3 cubeOrigin = origin.CubeCoords;
        Vector3 cubeDest = end.CubeCoords;
        Vector3 cubePos = cubeOrigin;

        DataItemTile nTile = DataItemWorld.main.GetTile(origin);
        List<DataItemTile> valid = new List<DataItemTile>();

        hitData.passible_location = nTile;
        float mult = 0;
        for (int I = 0; I <= data.maxrange; I++)
        {

            mult = I / data.maxrange;
            cubePos = Vector3.Lerp(origin.CubeCoords, end.CubeCoords, mult);


            foreach (DataItemTile n in nTile.neighbors)
            {
                float distA = end.DistanceFrom(n.coords);
                float distB = end.DistanceFrom(nTile.coords);

                if (distA < distB)
                {
                    nTile = n;
                }
                else if (distA == distB)
                {
                    var deltaA = n.coords.AxialCoords - end.AxialCoords;
                    var deltaB = nTile.coords.AxialCoords - end.AxialCoords;
                    if (Mathf.Abs(deltaA.x) < Mathf.Abs(deltaB.x) || Mathf.Abs(deltaA.y) < Mathf.Abs(deltaB.y))
                    {
                        nTile = n;
                    }
                }
            }

            

            if (!valid.Contains(nTile))
            {
                if (data.behavior == HitScanBehavior.Ghost)
                {
                    valid.Add(nTile);
                }
                else if (nTile.IsPassible())
                {
                    valid.Add(nTile);
                    if (nTile.LocatedEntity != null && data.stop_on_actor != Trajectory.ActorBehavior.never_stop || data.stop_on_object)
                    {
                        if (data.stop_on_object && nTile.LocatedEntity.IsInanimate())
                        {
                            break;
                        }
                        if (nTile.LocatedEntity != data.launcher && (
                                data.stop_on_actor == Trajectory.ActorBehavior.stop_all ||
                                (nTile.LocatedEntity.GetAlignment(data.launcher) == Mob.Alignment.enemy && data.stop_on_actor == Trajectory.ActorBehavior.stop_enemies) ||
                                (nTile.LocatedEntity.GetAlignment(data.launcher) > Mob.Alignment.enemy && data.stop_on_actor == Trajectory.ActorBehavior.stop_allies)
                                ))
                        {
                            break;
                        }
                    }
                    hitData.passible_location = nTile;
                }
                if (data.always_travel_max_distance && nTile.coords == end)
                {
                     end = new HexCoords( origin.AxialCoords + ((Vector2)end.AxialCoords - (Vector2)origin.AxialCoords).normalized * (data.maxrange * 2));
                }
            }
        }
        while (valid.Count > data.maxrange)
        {
            valid.RemoveAt(valid.Count - 1);
        }
        Debug.Log("[Pathfinder] " + data.behavior + " concluded.");
        hitData.trajectory = valid.ToArray();

        return hitData;
    }
}
