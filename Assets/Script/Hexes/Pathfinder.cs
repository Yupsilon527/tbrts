using UnityEngine;
using System.Collections;

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class Pathfinder
{
    public enum Failure
    {
        incomplete,
        impossible_outofrange,
        impassible_origin,
        impassible_target,
        impossible,
        success
    }

    public class Node
    {
        public HexCoords point;
        public float Distance;
        public int index;

        public Node(HexCoords point, float distance, int index)
        {
            this.point = point;
            Distance = distance;
            this.index = index;
        }
        public override string ToString()
        {
            return $"Node "+ point;
        }
    }

    int steps = 0;
    public int ApproachRange = 0;

    HexCoords hexOrigin;
    HexCoords hexDest;

    Mob entityParent;
    DataItemWorld grid;
    Node currentCell;

    bool includeOrigin = false;
    bool approximate = true;
    bool recalculate = true;
    bool staticOnly = true;

    List<Node> openList = new List<Node>();
    List<Node> closedList = new List<Node>();

    public class PathfinderPath
    {
        public int position = 0;
        public List<HexCoords> walkpath;
        public Failure FailureType;

        public PathfinderPath()
        {
            walkpath = new List<HexCoords>();
            FailureType = Failure.incomplete;
        }

        public void Cull(int desiredLength)
        {
            if (desiredLength>0 && walkpath.Count > desiredLength)
            {
                FailureType = Failure.impossible_outofrange;

                while (walkpath.Count > desiredLength)
                {
                    walkpath.RemoveAt(walkpath.Count - 1);
                }
            }
        }
    

        public HexCoords Current()
        {
            return walkpath[position];
        }
        public HexCoords Last()
        {
            if (walkpath.Count>0)
                return walkpath[walkpath.Count - 1];
            return null;
        }
        public HexCoords Next()
        {
            position++;
            if (Solved())
            {
                return walkpath[walkpath.Count - 1];
            }
            return walkpath[position];
        }
        public bool Solved()
        {
            return position >= walkpath.Count - 1;
        }
    }

    public Pathfinder(DataItemWorld g, Mob e, HexCoords origin, HexCoords dest)
    {
        grid = g;
        entityParent = e;
        this.hexOrigin = origin;
        this.hexDest = dest;
    }
    public Failure GetFailureType()
    {
        return failure;
    }

    public List<HexCoords> GetWalkPath(bool include_origin)
    {
        List<HexCoords> value = GetResult().walkpath;
        if (!include_origin)
        { value.RemoveAll((HexCoords T) => { return T .Equals( hexOrigin); }); }
        return value;
    }
    public PathfinderPath GetResult()
    {
        return finalPath;
    }

    public static PathfinderPath Solve(DataItemWorld g, Mob e, HexCoords origin, HexCoords dest, int aprRange)
    {
        Pathfinder pf = new Pathfinder(g,e,  origin, dest);
        pf.ApproachRange = aprRange;
        pf.solve();
        return pf.GetResult();
    }

    Failure failure = Failure.incomplete;
    public void solve()
    {

        if (hexDest .Equals( hexOrigin))
        {
            UnityEngine.Debug.LogWarning("Target Origin");
            return;
        }
        UnityEngine.Debug.Log("Initializing Pathfinder");
        currentCell = new Node(hexOrigin, hexOrigin.DistanceFrom (hexDest), 0);

        var originTile = grid.GetTile(hexOrigin);

        if (!WorldDefines.CanFitEntity(originTile, entityParent))
        {
            UnityEngine.Debug.LogWarning("Origin Impassible");
           // failure = Failure.impassible_origin;
         //   return;
        }
        openList.Add(currentCell);
        if (grid.GetTile(hexDest) == null || !WorldDefines.CanFitEntity(grid.GetTile(hexDest),entityParent) && ApproachRange == 0)
        {
            UnityEngine.Debug.LogWarning("Target Impassible " + ApproachRange);

            if (approximate)
            {
                hexDest = grid.GetClosestToPoint(entityParent,hexDest).coords;
            }
            else
            {
                failure = Failure.impassible_target;
            }
        }

        while (failure == Failure.incomplete)
        {
            stepPathfinder();
        }

        if (failure != Failure.success && recalculate)
        {
            recalculate = false;

            float sqrDist = Mathf.Infinity;
            Node newDest = null;
            foreach (Node n in closedList)
            {
                float dist = n.point.DistanceFrom( hexDest);

                var destTile = grid.GetTile(hexDest);
                if (dist < sqrDist && (destTile!=null && WorldDefines.CanFitEntity(destTile,entityParent)))
                {
                    newDest = n;
                    sqrDist = dist;
                }
            }
            openList.Clear();
            closedList.Clear();

            hexDest = newDest.point;
            solve();
        }
        else
        {
            ResolvePath();
            Debug.Log("Pathfinder concluded as " + GetFailureType());
        }
    }
    private void stepPathfinder()
    {
        if (currentCell.point.Equals( hexDest) || (ApproachRange > 0 && currentCell.point!=null && currentCell.point.DistanceFrom( hexDest) <= ApproachRange))
        {
            failure = Failure.success;
            return;
        }
                steps++;
        if (steps > grid.GetFullSize() * grid.GetFullSize())
        {
            failure = Failure.impassible_target;
            return;
        }

        foreach (HexCoords point in currentCell.point.GetNeighbors())
        {
            if (point != null)
            {
                var nTile = grid.GetTile(point);
                if (!WorldDefines.CanFitEntity(nTile,entityParent))
                    continue;
                
                bool exists = false;
                foreach (Node Zim in openList)
                {
                    if (Zim.point.Equals(point))
                    {
                        exists = true ;
                        break;
                    }
                }
                if (!exists)
                {
                    foreach (Node Gir in closedList)
                    {
                        if (Gir.point .Equals(point))
                        {
                            exists = true;
                            break;
                        }
                    }
                }
                if (!exists)
                {
                    openList.Add(new Node(point, hexDest.DistanceFrom(point), currentCell.index + 1));
                }
            }
        }

        if (openList.Count == 0)
        {
            failure = Failure.impossible;
        }
        else
        {
            closedList.Add(currentCell);
            currentCell = openList[0];
            openList.RemoveAt(0);
            openList.Sort((Node A, Node B) => { return A.Distance.CompareTo(B.Distance); });
        }
    }
    PathfinderPath finalPath = new PathfinderPath();
    void ResolvePath()
    {
        finalPath.FailureType = GetFailureType();
        if (finalPath.FailureType == Failure.success)
        {
            List<Node> walkPath = new List<Node>();
            walkPath.Add(currentCell);

            while (currentCell.index != 0)
            {
                foreach (HexCoords neighbor in currentCell.point.GetNeighbors())
                {
                    if (neighbor!=null )
                    {
                        DataItemTile neighborTile = grid.GetTile(neighbor);
                        if (neighborTile == null)
                        {
                            continue;
                        }
                        else if (WorldDefines.CanFitEntity(neighborTile,entityParent))
                        {
                            foreach (Node Gir in closedList)
                            {
                                if (Gir.point.Equals(neighbor) && Gir.index < currentCell.index)
                                {
                                    currentCell = Gir;
                                }
                            }
                        }
                    }
                }
                walkPath.Add(currentCell);
            }

            walkPath.Reverse();
            foreach (Node node in walkPath)
            {
                finalPath.walkpath.Add(node.point);
            }

            if (!includeOrigin)
                finalPath.walkpath.RemoveAt(0);

                //finalPath.Cull(MaxRange);
        }
    }

    public static HexCoords GetClosestPointForUnit(DataItemWorld grid, Mob unit, HexCoords center, float maxDistance)
    {
        HashSet<Node> valid = new HashSet<Node>();
        valid.Add(new Node(center, 0, 0));

        WorldDefines.SearchIndex++;

    loopstart:
        foreach (Node node in valid)
        {
            var tile = grid.GetTile(node.point);
            if (WorldDefines.CanFitEntity(tile, unit))
                return node.point;
            else
            {
                foreach (var neighbor in tile.neighbors)
                {
                    if (neighbor.searchIndex< WorldDefines.SearchIndex)
                    {
                        neighbor.searchIndex = WorldDefines.SearchIndex;
                        float dist = center.DistanceFrom(neighbor.coords);
                        if (dist <= maxDistance) valid.Add(new Node(neighbor.coords, dist, 0));
                    }
                }
                goto loopstart;
            }
        }

                return null;
    }
}