using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Astar
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
        public DataItemTile node;
        public float index = -1f;
        public float distance;
        public bool visited;
        public TerrainDefines.Elevation elevation;
        public Node[] neighbors;

        public Node(DataItemTile node) { this.node = node; elevation = node.GetWalkElevation(); }

        public override string ToString() => $"Node {node.gridPos}";

        public void Init(Pathfinder nav)
        {
            Vector2Int p = node.gridPos;
            neighbors = new Node[]
            {
                nav.GetNodeAt(p.x + 1, p.y + 1),
                nav.GetNodeAt(p.x,     p.y + 1),
                nav.GetNodeAt(p.x - 1, p.y + 1),
                nav.GetNodeAt(p.x - 1, p.y    ),
                nav.GetNodeAt(p.x + 1, p.y    ),
                nav.GetNodeAt(p.x + 1, p.y - 1),
                nav.GetNodeAt(p.x,     p.y - 1),
                nav.GetNodeAt(p.x - 1, p.y - 1),
            };
        }
    }

    public class PathfinderPath
    {
        public int position = 0;
        public List<DataItemTile> walkpath = new List<DataItemTile>();
        public Failure failure = Failure.incomplete;

        public void Cull(int desiredLength)
        {
            if (desiredLength > 0 && walkpath.Count > desiredLength)
            {
                failure = Failure.impossible_outofrange;
                walkpath.RemoveRange(desiredLength, walkpath.Count - desiredLength);
            }
        }

        public DataItemTile Current() => Following(0);
        public DataItemTile Following(int index) => walkpath.Count > 0 ? walkpath[Mathf.Clamp(index,0, walkpath.Count-1)] : null;
        public DataItemTile Last() => walkpath.Count > 0 ? walkpath[walkpath.Count - 1] : null;

        public DataItemTile Next()
        {
            if (walkpath == null || walkpath.Count == 0) return null;
            if (Solved()) return null;
            return walkpath[++position];
        }
        public int Remaining() =>  walkpath.Count  - position;
        
        public bool Solved() => Remaining()==0;
        public void Reset() { position = 0; failure = Failure.incomplete; }

        public DataItemTile GetIndex(int i) => walkpath[i];

        public DataItemTile[] GetIndex(int start, int count)
        {
            if (walkpath == null || walkpath.Count == 0) return new DataItemTile[0];
            int clampedStart = Mathf.Max(0, start);
            int clampedCount = Mathf.Min(count, walkpath.Count - clampedStart);
            if (clampedCount <= 0) return new DataItemTile[0];
            return walkpath.GetRange(clampedStart, clampedCount).ToArray();
        }
    }

    internal sealed class NodeHeap
    {
        private readonly List<Node> _heap = new List<Node>();

        public int Count => _heap.Count;

        public void Push(Node n)
        {
            _heap.Add(n);
            BubbleUp(_heap.Count - 1);
        }

        public Node Pop()
        {
            int last = _heap.Count - 1;
            Node top = _heap[0];
            _heap[0] = _heap[last];
            _heap.RemoveAt(last);
            if (_heap.Count > 0) SiftDown(0);
            return top;
        }

        public void Clear() => _heap.Clear();

        private bool Higher(Node a, Node b) =>
            a.index < b.index || (a.index == b.index && a.distance < b.distance);

        private void BubbleUp(int i)
        {
            while (i > 0)
            {
                int parent = (i - 1) >> 1;
                if (!Higher(_heap[i], _heap[parent])) break;
                (_heap[i], _heap[parent]) = (_heap[parent], _heap[i]);
                i = parent;
            }
        }

        private void SiftDown(int i)
        {
            int n = _heap.Count;
            while (true)
            {
                int best = i, l = (i << 1) + 1, r = l + 1;
                if (l < n && Higher(_heap[l], _heap[best])) best = l;
                if (r < n && Higher(_heap[r], _heap[best])) best = r;
                if (best == i) break;
                (_heap[i], _heap[best]) = (_heap[best], _heap[i]);
                i = best;
            }
        }
    }

    public class Pathfinder
    {
        public bool inspect = false;
        void Inspect(string msg) { if (inspect) Debug.Log($"[Inspect {mob.ToString()}] {msg}"); }

        int ApproachRange, MaxRange;
        public TerrainDefines.Movement movement;

        public Vector2Int vOrigin { get; private set; }
        public Vector2Int vDest { get; private set; }

        DataItemArmy mob;
        Node currentCell;

        // FIX D/E: renamed and semantically correct — stores walked nodes for closest-fallback search
        private readonly List<Node> _walkedNodes = new List<Node>();

        #region Node Grid
        Node[,] Nodes;
        const int ramLimit = 1000;

        void RedoGrid()
        {
            var g = SidewaysMap.main;
            int w = g.width, h = g.height;
            Nodes = new Node[w, h];
            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                {
                    Nodes[i, j] = new Node(SidewaysMap.main.GetTile(new Vector2Int(i, j)));
                }
            foreach (Node node in Nodes)
            {
                node.Init(this);
            }
            Inspect("Done Redo Grid");
        }
        public Node GetNodeAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Nodes.GetLength(0) || y >= Nodes.GetLength(1)) return null;
            return Nodes[x, y];
        }
        public Node GetNodeAt(Vector2Int pos) => GetNodeAt(pos.x, pos.y);
        #endregion

        #region Distance Data (Dijkstra flood-fill from destination)
        readonly NodeHeap _openHeap = new NodeHeap();

        void ReaccountDistanceData()
        {
            ClearDistanceData();
            Node dest = GetNodeAt(vDest);
            if (dest == null) { return; }

            dest.index = 0;
            dest.distance = 0;
            dest.visited = true;
            _openHeap.Clear();
            _openHeap.Push(dest);

            // FIX H: capture origin at BFS start, not from stale currentCell field
            Node originNode = GetNodeAt(vOrigin);

            while (_openHeap.Count > 0)
            {
                Node current = _openHeap.Pop();

                if (current == originNode) break;

                float nextIndex = current.index + current.node.GetMoveCost(movement);
                Node[] neighbors = current.neighbors;
                for (int i = 0; i < neighbors.Length; i++)
                {
                    Node nb = neighbors[i];
                    if (nb == null || !nb.node.IsPassible(movement)) continue;
                    if (nb.visited) continue;

                    float dx = vDest.x - nb.node.gridPos.x;
                    float dy = vDest.y - nb.node.gridPos.y;
                    nb.distance = dx * dx + dy * dy;
                    nb.index = nextIndex;
                    nb.visited = true;
                    _openHeap.Push(nb);
                }

            }
        }

        void ClearDistanceData()
        {
            foreach (Node n in Nodes) { n.index = -1f; n.visited = false; }
        }
        #endregion

        #region Init & Solve
        public Pathfinder(DataItemArmy a)
        {
            mob = a;
            movement = a.movement.GetMyMovement();
            RedoGrid();
        }

        public PathfinderPath path { get; private set; }

        public PathfinderPath Solve(Vector2Int o, Vector2Int d)
        {
            path = null;
            vDest = d;
            vOrigin = o;
            ReaccountDistanceData();
            return Solution(o);
        }

         PathfinderPath Solution(
            Vector2Int origin,
            bool includeOrigin = true,
            bool approximate = true,
            bool recalculate = true,
            bool account_entities = false,
            int approach = 0,
            int maxrange = 0)
        {
            vOrigin = origin;
            ApproachRange = approach;
            MaxRange = maxrange;

            if (vDest.Equals(vOrigin))
                return ResolvePath(new List<Node>(), includeOrigin, Failure.success);

            currentCell = GetNodeAt(vOrigin);
            if (currentCell == null || !TerrainDefines.CanIWalkOver(movement, currentCell.elevation))
                return ResolvePath(new List<Node> { currentCell }, includeOrigin, Failure.impassible_origin);

            Node destNode = GetNodeAt(vDest);
            if (destNode == null || !TerrainDefines.CanIWalkOver(movement, destNode.elevation))
            {
                if (ApproachRange == 0)
                {
                    if (approximate)
                        vDest = SidewaysMap.main.GetClosestToPoint(vDest, movement).gridPos;
                    else
                        return ResolvePath(new List<Node> { currentCell }, includeOrigin, Failure.impassible_target);
                }
            }

            List<Node> walkPath = new List<Node> { currentCell };
            _walkedNodes.Clear();
            Failure failure = Failure.incomplete;
            int loop = 1000;

            while (failure == Failure.incomplete && --loop > 0)
            {
                failure = StepPathfinder(account_entities, out Node next);
                if (next != null)
                {
                    currentCell = next;
                    walkPath.Add(next);
                    _walkedNodes.Add(next);
                    if (MaxRange > 0 && walkPath.Count > MaxRange)
                        return ResolvePath(walkPath, includeOrigin, Failure.impossible_outofrange);
                }
                else break;
            }

            if (failure != Failure.success && recalculate)
            {
                float sqrDist = float.MaxValue;
                Node newDest = null;

                foreach (Node n in _walkedNodes)
                {
                    if (n.node.armyLayer != mob) continue;
                    float dist = (n.node.gridPos - vDest).sqrMagnitude;
                    if (dist < sqrDist)
                    {
                        newDest = n;
                        sqrDist = dist;
                    }
                }

                if (newDest != null)
                {
                    vDest = newDest.node.gridPos;
                    _walkedNodes.Clear();
                    return Solution(origin, includeOrigin, approximate, false, account_entities, approach, maxrange);
                }
            }

            return ResolvePath(walkPath, includeOrigin, failure);
        }

        PathfinderPath ResolvePath(List<Node> walkPath, bool includeOrigin, Failure failure)
        {
            var finalPath = new PathfinderPath { failure = failure };
            for (int i = 0; i < walkPath.Count; i++)
            {
                if (walkPath[i] == null) continue;
                if (!includeOrigin && walkPath[i].node.gridPos == vOrigin) continue;
                finalPath.walkpath.Add(walkPath[i].node);
            }
            return finalPath;
        }

        Failure StepPathfinder(bool accountEntities, out Node nextPos)
        {
            nextPos = NextStep(accountEntities);
            if (nextPos == null) return Failure.impossible;
            if (nextPos.node.gridPos.Equals(vDest) ||
                (ApproachRange > 0 && (nextPos.node.gridPos - vDest).sqrMagnitude <= ApproachRange * ApproachRange))
                return Failure.success;
            return Failure.incomplete;
        }

        Node NextStep(bool accountEntities)
        {
            if (currentCell?.neighbors == null)
            {
                Inspect("Error! Cell or neighbors null!");
                return null;
            }

            Node best = null;
            Node[] neighbors = currentCell.neighbors;

            for (int i = 0; i < neighbors.Length; i++)
            {
                Node nb = neighbors[i];
                if (nb == null || nb.index < 0 ) continue;
                if (accountEntities && nb.node.armyLayer != mob) continue;

                if (nb.index == 0 || _walkedNodes.Contains(nb)) return nb;

                if (best == null
                    || nb.index < best.index
                    || (nb.index == best.index && nb.distance < best.distance))
                    best = nb;
            }
            return best;
        }
        #endregion
    }
}