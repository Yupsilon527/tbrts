using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCoords
{
    Vector3Int cubev;
    Vector2Int axialv;
    public Vector2 worldPosition
    {
        get
        {
            return DataItemWorld.main.TranslatePositionFromCenter(this);
        }
    }

    public Vector3Int CubeCoords
    {
        get { return cubev; }
        set
        {
            cubev = value;
            axialv = CubeToAxial(cubev);
        }
    }
    public Vector2Int AxialCoords
    {
        get { return axialv; }
        set
        {
            axialv = value;
            cubev = AxialToCube(axialv);
        }
    }
    public Vector2Int GlobalAxialCoords
    {
        get
        {
            if (DataItemWorld.main != null)
            {
                return DataItemWorld.main.Globalize(axialv);
            }

            return axialv;
        }
    }
    public float DistanceFrom(HexCoords other, bool global = true)
    {
        if (!global)
        return AxialDistance(axialv, other.axialv);
    else
        return CubeGlobalDistance(cubev, other.cubev,DataItemWorld.main.GetFullSize());
    }
    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            HexCoords p = (HexCoords)obj;
            
                return GlobalAxialCoords == p.GlobalAxialCoords;
        }
    }
    public override string ToString()
    {
        return $"Hex<{axialv.x},{axialv.y}> ({GlobalAxialCoords.x},{GlobalAxialCoords.y})";
    }
    public HexCoords[] GetNeighbors()
    {
        return new HexCoords[]
        {
            new HexCoords(new Vector2Int(axialv.x + 1,axialv.y - 1)),
            new HexCoords(new Vector2Int(axialv.x ,axialv.y - 1)),
            new HexCoords(new Vector2Int(axialv.x + 1,axialv.y)),
        new HexCoords(new Vector2Int(axialv.x - 1, axialv.y )),
        new HexCoords(new Vector2Int(axialv.x - 1,axialv.y + 1)),
        new HexCoords(new Vector2Int(axialv.x, axialv.y + 1)),
        };
    }
    public HexCoords(Vector3Int CubeCoords)
    {
        this.CubeCoords = CubeCoords;
    }
    public HexCoords(Vector2Int AxialCoords)
    {
        this.AxialCoords = AxialCoords;
    }
    public HexCoords(Vector3 CubeCoords)
    {
        this.CubeCoords = Vector3Int.RoundToInt( CubeCoords);
    }
    public HexCoords(Vector2 AxialCoords)
    {
        this.AxialCoords = Vector2Int.RoundToInt(AxialCoords);
    }

    public static float AxialDistance(Vector2Int start, Vector2Int dest)
    {
        Vector2Int delta = dest - start;

        return (Mathf.Abs(delta.x) + Mathf.Abs(delta.x + delta.y) + Mathf.Abs(delta.y)) * .5f;
    }
    public static float CubeDistance(Vector3Int start, Vector3Int dest)
    {
        Vector3Int delta = dest - start;
        return Mathf.Max(delta.x, delta.y, delta.z);
    }
    public static float CubeGlobalDistance(Vector3Int start, Vector3Int dest, int worldSize)
    {
        float width = worldSize;
        float height = worldSize;
        Vector3 delta = dest - start;

        delta.x = Mathf.Abs(delta.x);
        delta.y = Mathf.Abs(delta.y);
        delta.z = Mathf.Abs(delta.z);

        if (delta.x > width / 2)
        {
            delta.x = width - delta.x;
        }
        if (delta.y > height / 2)
        {
            delta.y = height - delta.y;
        }
        if (delta.z > height / 2)
        {
            delta.z = height - delta.z;
        }


        return Mathf.Max(Mathf.Abs(delta.x), Mathf.Abs(delta.y), Mathf.Abs(delta.z));
    }
    public static bool IsAdjencent(Vector3Int start, Vector3Int dest)
    {
        Vector3Int delta = dest - start;
        return Mathf.Abs(delta.x) == 1 || Mathf.Abs(delta.y) == 1 || Mathf.Abs(delta.z) == 1;
    }

    #region Cube Coords
    public static Vector2Int CubeToAxial(Vector3Int coords)
    {
        return new Vector2Int(coords.x, coords.y);
    }
    public static Vector2Int CubeToAxial(Vector3 coords)
    {
        return CubeToAxial(Vector3Int.RoundToInt(coords));
    }
    public static Vector3Int AxialToCube(Vector2Int coords)
    {
        return new Vector3Int(coords.x, coords.y, -coords.x - coords.y);
    }
    public static Vector3Int AxialToCube(Vector2 coords)
    {
        return AxialToCube(Vector2Int.RoundToInt(coords));
    }
    public static float CubeDist(Vector3Int vA, Vector3Int vB)
    {
        return Mathf.Max(Mathf.Abs(vA.x - vB.x), Mathf.Abs(vA.y - vB.y), Mathf.Abs(vA.z - vB.z));
    }
    #endregion
}