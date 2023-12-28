using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNav : MonoBehaviour
{
    public float UnitSize = 1;

    Node[,] Nodes;

    public class Node
    {
        public Vector2Int gridPos;
        public Vector2 worldPos;
        public bool passible;
        public Node[] neighbors;

        public Node(Vector2Int gridPos, Vector2 worldPos)
        {
            this.gridPos = gridPos;
            this.worldPos = worldPos;
        }

        public bool IsNeighboring(Node other)
        {
            return Mathf.Abs(other.gridPos.x - gridPos.x) <= 1 && Mathf.Abs(other.gridPos.y - gridPos.y) <= 1;
        }


        public void UpdateNeighbors(GridNav nav)
        {
            neighbors = new Node[]{
                nav.GetNodeAt(gridPos + Vector2Int.right + Vector2Int.up),
                nav.GetNodeAt(gridPos +  Vector2Int.up),
                nav.GetNodeAt(gridPos + Vector2Int.left + Vector2Int.up),
                nav.GetNodeAt(gridPos +  Vector2Int.left),
                nav.GetNodeAt(gridPos +  Vector2Int.right),
                nav.GetNodeAt(gridPos + Vector2Int.right + Vector2Int.down),
                nav.GetNodeAt(gridPos +  Vector2Int.down),
                nav.GetNodeAt(gridPos + Vector2Int.left + Vector2Int.down),
                };
        }
        public void UpdateCollision(GridNav nav)
        {
            passible = true;
            foreach (RaycastHit2D collision in Physics2D.CircleCastAll(worldPos, nav.UnitSize*.5f, Vector2.zero) )
            {
                if (collision.collider.enabled && collision.transform.CompareTag("Obstacle"))
                {
                    passible = false;
                }
            }
        }

        public Mob LocatedMob;
    }
    private void Awake()
    {
        InitGrid();
    }
    int width;
    int height;
    Vector2 origin;
    void InitGrid()
    {
        width = Mathf.CeilToInt( transform.lossyScale.x / UnitSize);
        height = Mathf.CeilToInt(transform.lossyScale.y / UnitSize);
        origin = new Vector2(transform.position.x - transform.lossyScale.x * .5f, transform.position.y - transform.lossyScale.y * .5f);

        Nodes = new Node[width, height];
        for (int iX = 0; iX < width; iX++)
        {
            for (int iY = 0; iY < height; iY++)
            {
                Nodes[iX, iY] = new Node(new (iX,iY), new Vector2(origin.x + iX * UnitSize, origin.y + iY * UnitSize));
            }
        }
        foreach (Node n in Nodes)
        {
            n.UpdateCollision(this);
            n.UpdateNeighbors(this);
        }
    }
    public int GetWidth()
    {
        return width;
    }
    public int GetHeight()
    {
        return height;
    }
    public bool TryGetWorldNodeAt(Vector2 pos, out Node node)
    {
        return TryGetNodeAt(TranslateCoordinate(pos), out node);
    }
    public bool TryGetNodeAt(Vector2Int pos, out Node node)
    {
        node = GetNodeAt(pos);
        return node != null;
    }
    public Node GetNodeAt(Vector2Int pos)
    {
        if (Nodes == null || pos.x < 0 || pos.y < 0 || pos.x >= Nodes.GetLength(0) || pos.y >= Nodes.GetLength(1))
            return null;
        return Nodes[pos.x, pos.y];
    }
    void OnDrawGizmos()
    {
        if (Nodes!=null)
        foreach (Node n in Nodes)
        {
            Gizmos.color = n.passible ? (n.LocatedMob == null ? Color.green : Color.yellow) : Color.red;
            Gizmos.DrawSphere(n.worldPos, .1f);
        }
    }
    public Node GetClosestToPoint(Vector2Int point)
    {
        float sqrDist = Mathf.Infinity;
        Node dest = null;
        foreach (Node n in Nodes)
        {
            if (n.passible)
            {
                float dist = (n.gridPos - point).sqrMagnitude;
                if (dist < sqrDist)
                {
                    dest = n;
                    sqrDist = dist;
                }
            }
        }
        return dest;
    }
    public Vector2Int TranslateCoordinate(Vector2 point)
    {
        return Vector2Int.CeilToInt((point - origin - Vector2.one * .5f * UnitSize) / UnitSize);
    }
    public Node[] GetNodesInCircle(Vector2 center, float radius)
    {
        return GetNodesInCircle(TranslateCoordinate(center), Mathf.RoundToInt(radius / UnitSize));
    }
    public Node[] GetNodesInCircle(Vector2Int center, int radius)
    {
        Vector2Int start = center - radius * Vector2Int.one;
        Vector2Int end = center + radius * Vector2Int.one;

        List<Node> nodes = new List<Node>();
        for (Vector2Int pos = start; pos.y <= end.y; pos.x++)
        {
            if ((pos - center).sqrMagnitude <= radius * radius)
            {
                var node = GetNodeAt(pos);
                if (node != null) nodes.Add(node);
            }
            if (pos.x > end.x)
            { pos.x = start.x; pos.y++; 
            
            }
        }
        return nodes.ToArray();
    }
    public Node[] GetNodesInBox(Vector2 start, Vector2 end)
    {
        return GetNodesInBox(TranslateCoordinate(start),TranslateCoordinate(end));
    }
    public Node[] GetNodesInBox(Vector2Int start, Vector2Int end)
    {
        start = new Vector2Int(
            Mathf.Clamp(start.x-1, 0, width),
            Mathf.Clamp(start.y-1, 0, height)
            );

        end = new Vector2Int(
            Mathf.Clamp(end.x+1, 0, width),
            Mathf.Clamp(end.y+1, 0, height)
            );

        List<Node> nodes = new List<Node>();    
        for (Vector2Int pos = start; pos.y <= end.y; pos.x++)
        {
            var node = GetNodeAt(pos);
            if (node!=null) nodes.Add(node);
            if (pos.x > end.x)
            { pos.x = start.x; pos.y++; 
            }
        }
        return nodes.ToArray();
    }
    public Node RandomNodeInCircle(Vector2 center, float radius, bool passible = true, bool empty = true)
    {

        List<GridNav.Node> nodes = new List<GridNav.Node>();
        nodes.AddRange(GetNodesInCircle(center, radius));
        nodes.RemoveAll((GridNav.Node n) =>
        {
            return n == null || (passible && !n.passible) || (empty && n.LocatedMob !=null);
        });
        if (nodes.Count > 0)
            return nodes[Random.Range(0, nodes.Count)];
        return null;
    }
}
