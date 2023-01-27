using System.Collections.Generic;
using UnityEngine;

public class BlockMesh
{
    public Dictionary<Direction, Vector3[]> Vertices { get; protected set; }
    public Dictionary<Direction, Vector2[]> Uvs { get; protected set; }
    public Dictionary<Direction, int[]> Indices { get; protected set; }
    public Dictionary<Direction, Vector3> Normals { get; protected set; }

    public static readonly BlockMesh CubeMesh = new CubeMesh();
    public static readonly BlockMesh BottomSlopeMesh = new BottomSlopeMesh();
    public static readonly BlockMesh TopSlopeMesh = new TopSlopeMesh();

    private static readonly Vector3 Half = Vector3.one * 0.5f;

    public static Vector3 Rotate(Vector3 v, Quaternion q)
    {
        return q * (v - Half) + Half;
    }
}