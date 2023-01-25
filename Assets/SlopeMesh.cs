using System.Collections.Generic;
using UnityEngine;

public class SlopeMesh : BlockMesh
{
    public SlopeMesh()
    {
        Vertices = new Dictionary<Direction, Vector3[]>
        {
            {
                Direction.ZPos,
                new[]
                {
                    new Vector3(0, 0, 1),
                    new Vector3(0, 1, 1),
                    new Vector3(1, 1, 1),
                    new Vector3(1, 0, 1)
                }
            },
            {
                Direction.ZNeg,
                new Vector3[] {}
            },
            {
                Direction.XPos,
                new[]
                {
                    new Vector3(1, 0, 0),
                    new Vector3(1, 0, 1),
                    new Vector3(1, 1, 1),
                    new Vector3(1, 0, 0)
                }
            },
            {
                Direction.XNeg,
                new[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(0, 0, 1),
                    new Vector3(0, 1, 1),
                    new Vector3(0, 0, 0)
                }
            },
            {
                Direction.YPos,
                new[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(0, 1, 1),
                    new Vector3(1, 1, 1),
                    new Vector3(1, 0, 0)
                }
            },
            {
                Direction.YNeg,
                new[]
                {
                    new Vector3(0, 0, 0),
                    new Vector3(0, 0, 1),
                    new Vector3(1, 0, 1),
                    new Vector3(1, 0, 0)
                }
            }
        };
        
        Uvs = new Dictionary<Direction, Vector2[]>
        {
            {
                Direction.ZPos,
                new[]
                {
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 1)
                }
            },
            {
                Direction.ZNeg,
                new Vector2[] {}
            },
            {
                Direction.XPos,
                new[]
                {
                    new Vector2(1, 1),
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(1, 0)
                }
            },
            {
                Direction.XNeg,
                new[]
                {
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                    new Vector2(0, 0)
                }
            },
            {
                Direction.YPos,
                new[]
                {
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1)
                }
            },
            {
                Direction.YNeg,
                new[]
                {
                    new Vector2(0, 1),
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(1, 1)
                }
            }
        };

        Indices = new Dictionary<Direction, int[]>
        {
            {
                Direction.ZPos,
                new[] { 0, 2, 1, 0, 3, 2 }
            },
            {
                Direction.ZNeg,
                new int[] {}
            },
            {
                Direction.XPos,
                new[] { 0, 2, 1, 0, 3, 2 }
            },
            {
                Direction.XNeg,
                new[] { 0, 1, 2, 0, 2, 3 }
            },
            {
                Direction.YPos,
                new[] { 0, 1, 2, 0, 2, 3 }
            },
            {
                Direction.YNeg,
                new[] { 0, 2, 1, 0, 3, 2 }
            }
        };
        
        Normals = new Dictionary<Direction, Vector3>
        {
            {
                Direction.ZPos,
                Direction.ZPos.ToVec()
            },
            {
                Direction.ZNeg,
                Direction.ZNeg.ToVec()
            },
            {
                Direction.XPos,
                Direction.XPos.ToVec()
            },
            {
                Direction.XNeg,
                Direction.XNeg.ToVec()
            },
            {
                Direction.YPos,
                new Vector3(0, 1, -1).normalized
            },
            {
                Direction.YNeg,
                Direction.YNeg.ToVec()
            }
        };
    }
}