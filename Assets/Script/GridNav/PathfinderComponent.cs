using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfinderComponent : MobComponent
{
    public GridNav grid;

    public class DynamicNode
    {
        public GridNav.Node node;
        public float index;
        public float distance;

        public DynamicNode(GridNav.Node node)
        {
            this.node = node;
        }
        public DynamicNode[] neighbors;
        public bool IsPassible()
        {
            return node.passible;
        }
        public void Init(PathfinderComponent nav)
        {
            neighbors = new DynamicNode[]{
                nav.GetNodeAt(node.gridPos + Vector2Int.right + Vector2Int.up),
                nav.GetNodeAt(node.gridPos +  Vector2Int.up),
                nav.GetNodeAt(node.gridPos + Vector2Int.left + Vector2Int.up),
                nav.GetNodeAt(node.gridPos +  Vector2Int.left),
                nav.GetNodeAt(node.gridPos +  Vector2Int.right),
                nav.GetNodeAt(node.gridPos + Vector2Int.right + Vector2Int.down),
                nav.GetNodeAt(node.gridPos +  Vector2Int.down),
                nav.GetNodeAt(node.gridPos + Vector2Int.left + Vector2Int.down),
                };
        }
    }
    DynamicNode[,] dynamicNodes;
    public void ChangeGrid(GridNav g, GridNav.Node point)
    {
        grid = g;
        RedoGrid();
        MoveToNode(point);
        transform.position = occupyingNode.worldPos;
        if (parent.movement != null) parent.movement.AdjustZlevel();

    }
    void RedoGrid()
    {
        dynamicNodes = new DynamicNode[grid.GetWidth(), grid.GetHeight()];
        for (int i = 0; i < dynamicNodes.GetLength(0); i++)
        {
            for (int j = 0; j < dynamicNodes.GetLength(1); j++)
            {
                dynamicNodes[i, j] = new DynamicNode(grid.GetNodeAt(new Vector2Int(i, j)));
            }
        }
        foreach (DynamicNode node in dynamicNodes)
        {
            node.Init(this);
        }
        ClearDistanceData();
    }
    public DynamicNode GetNodeAt(Vector2Int pos)
    {
        if (pos.x < 0 || pos.y < 0 || pos.x >= dynamicNodes.GetLength(0) || pos.y >= dynamicNodes.GetLength(1))
            return null;
        return dynamicNodes[pos.x, pos.y];
    }
    Vector2Int origin;
    Vector2Int dest;
    
    public void Resolve(Vector2 dest, bool closest = false)
    {
        if (closest)
            Resolve(grid.GetClosestToPoint(grid.TranslateCoordinate(dest)));
        else
        Resolve(grid.TranslateCoordinate(dest));
    }
    public void Resolve(GridNav.Node d)
    {
        Resolve(d.gridPos);
    }
    public void Resolve(Vector2Int d)
    {
        dest = d;
        followTarget = null;
        solve();
    }

    Vector2 lastTargetPos;
    GameObject followTarget ;
    public void Follow (GameObject target)
    {
        followTarget = target;
        MoveToTarget();
    }
    void OnTargetMove()
    {
        if(followTarget!=null && ((Vector2)followTarget.transform.position - lastTargetPos).sqrMagnitude > 1)//TODO define
        { MoveToTarget(); }
    }
    void MoveToTarget()
    {
        lastTargetPos = transform.position;

        dest = grid.TranslateCoordinate(followTarget.transform.position);

        solve();
    }
    
    void solve()
    {
        ClearDistanceData();
        if (GetNodeAt(dest) == null)
        {
            progress = Failure.impassible_target;
            return;
        }
        progress = Failure.incomplete;
        ReaccountDistanceData();
    }

    void ClearDistanceData()
    {
        foreach (DynamicNode node in dynamicNodes)
        {
            node.index = -1;
        }
    }
    List<DynamicNode> openList = new List<DynamicNode>();
    void ReaccountDistanceData()
    {
        DynamicNode origin = GetNodeAt(dest);
        origin.index = 0;

        openList.Clear();
        openList.Add(origin);
        while (openList.Count > 0)
        {
            // Find the node with the lowest index in the open list
            float lowestIndex = int.MaxValue;
            DynamicNode lowestIndexNode = null;

            foreach (DynamicNode node in openList)
            {
                if (node.index < lowestIndex)
                {
                    lowestIndex = node.index;
                    lowestIndexNode = node;
                }
            }

            if (lowestIndexNode == null)
            {
                break; // No more accessible nodes in the open list
            }

            openList.Remove(lowestIndexNode);

            DynamicNode currentNode = lowestIndexNode;
            foreach (DynamicNode neighbor in currentNode.neighbors)
            {
                if (neighbor != null && neighbor.IsPassible() && (neighbor.index < 0 || neighbor.index > currentNode.index + 1))
                {
                    neighbor.distance = new Vector2(dest.x - neighbor.node.gridPos.x, dest.y - neighbor.node.gridPos.y).sqrMagnitude;
                    neighbor.index = currentNode.index + 1;
                    openList.Add(neighbor);
                }
            }
        }
    }

    public DynamicNode positionNode;

    public enum Failure
    {
        incomplete,
        impossible_outofrange,
        impassible_target,
        impossible,
        success
    }
    public Failure progress;
    public Failure Step()
    {
        OnTargetMove();
        if (positionNode.index == 0)
        {
            progress = Failure.success;
        }
        else
        {
            DynamicNode nextStep = NextStep();
            if (nextStep != null)
            {
                MoveToNode(nextStep);
                progress = Failure.incomplete;
            }
            else
            {
                progress = Failure.impassible_target;
            }
        }
        return progress;
    }
    public void MoveToNode(GridNav.Node pos)
    {
        if (pos!=null)
        MoveToNode(pos.gridPos);
    }
    public void MoveToNode(Vector2Int pos)
    {
        MoveToNode(GetNodeAt(pos));
    }
    public void MoveToNode(DynamicNode pos)
    {
        positionNode = pos;
        ChangeNode(pos.node);
    }
    DynamicNode NextStep()
    {
        if (positionNode != null)
        {
            DynamicNode nextNode = null;
            foreach (DynamicNode pickedNode in positionNode.neighbors)
            {
                if (pickedNode != null)
                {
                    if (pickedNode.index == 0)
                    {
                        if (pickedNode.node.LocatedMob == null)
                            return pickedNode;
                        else 
                            return null;
                    }
                    else if (pickedNode.index > 0 && pickedNode.node.LocatedMob == null)
                    {
                        if (nextNode == null || (pickedNode.index <= nextNode.index  && pickedNode.distance< nextNode.distance) )
                        {
                            nextNode = pickedNode;
                        }
                    }
                }
            }
            return nextNode;
        }
        return null;
    }
    GridNav.Node occupyingNode;
    void ChangeNode(GridNav.Node node)
    {
        ClearNode();
        occupyingNode = node;
        occupyingNode.LocatedMob = parent;
    }
    public void ClearNode()
    {
        if (occupyingNode != null && occupyingNode.LocatedMob == parent)
        {
            occupyingNode.LocatedMob = null;
        }
    }
}

