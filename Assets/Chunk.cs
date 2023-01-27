using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    public const int Size = 32;

    public World world;
    public Vector3Int chunkPosition;
    public BlockTextureArray BlockTextureArray;

    public MeshFilter solidMeshFilter;
    public MeshRenderer solidMeshRenderer;
    
    public MeshFilter transparentMeshFilter;
    public MeshRenderer transparentMeshRenderer;

    private Mesh _transparentMesh;
    private Mesh _solidMesh;

    private Block[] _blocks;
    private Direction[] _blockOrientations;
    private bool _isDirty;
    
    private List<Vector3> _solidVertices;
    private List<Vector3> _solidNormals;
    private List<Vector3> _solidUvs;
    private List<int> _solidIndices;
    
    private List<Vector3> _transparentVertices;
    private List<Vector3> _transparentNormals;
    private List<Vector3> _transparentUvs;
    private List<int> _transparentIndices;

    public void Initialize()
    {
        _solidMesh = new Mesh
        {
            indexFormat = IndexFormat.UInt32
        };
        solidMeshFilter.mesh = _solidMesh;
        solidMeshRenderer.material = BlockTextureArray.SolidMaterial;
        
        _transparentMesh = new Mesh
        {
            indexFormat = IndexFormat.UInt32
        };
        transparentMeshFilter.mesh = _transparentMesh;
        transparentMeshRenderer.material = BlockTextureArray.TransparentMaterial;

        _blocks = new Block[Size * Size * Size];
        _blockOrientations = new Direction[Size * Size * Size];
        Array.Fill(_blockOrientations, Direction.ZPos);

        _solidVertices = new List<Vector3>();
        _solidNormals = new List<Vector3>();
        _solidUvs = new List<Vector3>();
        _solidIndices = new List<int>();
        
        _transparentVertices = new List<Vector3>();
        _transparentNormals = new List<Vector3>();
        _transparentUvs = new List<Vector3>();
        _transparentIndices = new List<int>();
    }

    private void Update()
    {
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        if (!_isDirty) return;
        GenerateMesh();
        _isDirty = false;
    }

    private void TestFill()
    {
        for (var z = 0; z < Size; z++)
        for (var y = 0; y < Size; y++)
        for (var x = 0; x < Size; x++)
        {
            var position = new Vector3Int(x, y, z);
            if (Random.Range(0, 2) == 1) SetBlock(Block.Tile, position);
        }
    }

    private void GenerateMesh()
    {
        for (var z = 0; z < Size; z++)
        for (var y = 0; y < Size; y++)
        for (var x = 0; x < Size; x++)
        {
            var position = new Vector3Int(x, y, z);
            var block = GetBlock(position);
            if (block == Block.Air) continue;
            var blockData = BlockData.Registry[block];
            var blockOrientation = GetBlockOrientation(position);

            GenerateFace(block, blockOrientation, blockData, position, Direction.XPos);
            GenerateFace(block, blockOrientation, blockData, position, Direction.XNeg);
            GenerateFace(block, blockOrientation, blockData, position, Direction.YPos);
            GenerateFace(block, blockOrientation, blockData, position, Direction.YNeg);
            GenerateFace(block, blockOrientation, blockData, position, Direction.ZPos);
            GenerateFace(block, blockOrientation, blockData, position, Direction.ZNeg);
        }

        _solidMesh.Clear();
        _solidMesh.SetVertices(_solidVertices);
        _solidMesh.SetIndices(_solidIndices, MeshTopology.Triangles, 0);
        _solidMesh.SetNormals(_solidNormals);
        _solidMesh.SetUVs(0, _solidUvs);
        
        _transparentMesh.Clear();
        _transparentMesh.SetVertices(_transparentVertices);
        _transparentMesh.SetIndices(_transparentIndices, MeshTopology.Triangles, 0);
        _transparentMesh.SetNormals(_transparentNormals);
        _transparentMesh.SetUVs(0, _transparentUvs);
    }

    private void GenerateFace(Block block, Direction blockOrientation, BlockData blockData, Vector3Int localPosition,
        Direction direction)
    {
        var vertices = _solidVertices;
        var normals = _solidNormals;
        var uvs = _solidUvs;
        var indices = _solidIndices;
        
        var directionVec = direction.ToVec();
        var worldPosition = localPosition + chunkPosition;
        switch (blockData.MeshType)
        {
            case BlockMeshType.Opaque:
                if (world.GetBlock(worldPosition + directionVec).IsOpaque()) return;
                break;
            case BlockMeshType.Transparent:
                vertices = _transparentVertices;
                normals = _transparentNormals;
                uvs = _transparentUvs;
                indices = _transparentIndices;
                
                var adjacentBlock = world.GetBlock(worldPosition + directionVec);
                if (adjacentBlock == block || adjacentBlock.IsOpaque()) return;
                break;
            case BlockMeshType.Model:
            default:
                break;
        }

        var vertexCount = vertices.Count;
        var faceIndices = blockData.Mesh.Indices[direction];

        for (var i = 0; i < faceIndices.Length; i++) indices.Add(faceIndices[i] + vertexCount);

        var rotation = blockOrientation.ToRotation();
        var faceVertices = blockData.Mesh.Vertices[direction];
        var faceNormal = BlockMesh.Rotate(blockData.Mesh.Normals[direction], rotation);

        for (var i = 0; i < faceVertices.Length; i++)
        {
            var baseVertexPosition = BlockMesh.Rotate(faceVertices[i], rotation);
            var offset = PseudoRandomFloat(baseVertexPosition + worldPosition * World.BlockScale) * 0.1f;
            vertices.Add(baseVertexPosition + localPosition * World.BlockScale + new Vector3(offset, offset, offset));
            normals.Add(faceNormal);
        }

        var faceUvs = blockData.Mesh.Uvs[direction];
        var textureIndex = BlockTextureArray.GetTextureIndex(block, direction);

        for (var i = 0; i < faceUvs.Length; i++) uvs.Add(new Vector3(faceUvs[i].x, faceUvs[i].y, textureIndex));
    }
    
    public static float PseudoRandomFloat(Vector3 position)
    {
        var vx = (int)position.x;
        var vy = (int)position.y;
        var vz = (int)position.z;
        
        var smallPos = new Vector3(MathF.Sin(vx), MathF.Sin(vy), MathF.Sin(vz));

        var random = Vector3.Dot(smallPos, new Vector3(12.9898f, 78.233f, 37.719f));
        random = MathF.Sin(random) * 143758.5453f;
        random -= MathF.Floor(random);
        return random;
    }

    public Block GetBlock(Vector3Int position)
    {
        if (position.x is < 0 or >= Size || position.y is < 0 or >= Size || position.z is < 0 or >= Size)
            return Block.Barrier;
        return _blocks[position.x + position.y * Size + position.z * Size * Size];
    }

    public Direction GetBlockOrientation(Vector3Int position)
    {
        if (position.x is < 0 or >= Size || position.y is < 0 or >= Size || position.z is < 0 or >= Size)
            return Direction.ZPos;
        return _blockOrientations[position.x + position.y * Size + position.z * Size * Size];
    }

    public void SetBlock(Block block, Vector3Int position, Direction orientation = Direction.ZPos)
    {
        if (position.x is < 0 or >= Size || position.y is < 0 or >= Size || position.z is < 0 or >= Size) return;
        var index = position.x + position.y * Size + position.z * Size * Size;
        _blocks[index] = block;
        _blockOrientations[index] = orientation;
        _isDirty = true;
    }
}